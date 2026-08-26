using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace XClone.Api.Services;

public class S3FileService : IFileService
{
    private readonly IConfiguration _configuration;
    
    public S3FileService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<string> UploadFileAsync(IFormFile file, string fileName)
    {
        var endpoint = _configuration["MinIO:Endpoint"];
        var accessKey = _configuration["MinIO:AccessKey"];
        var secretKey = _configuration["MinIO:SecretKey"];
        var bucketName = _configuration["MinIO:BucketName"];
        
        var credentials = new BasicAWSCredentials(accessKey, secretKey);
        
        var config = new AmazonS3Config
        {
            ServiceURL = endpoint,
            // 🚨 ВАЖНЕЙШАЯ НАСТРОЙКА ДЛЯ MINIO: ForcePathStyle = true
            // Amazon по умолчанию делает запросы так: http://avatars.aws.com/
            // А MinIO требует так: http://localhost:9000/avatars/
            // Эта галочка переключает клиента в нужный нам режим!
            ForcePathStyle = true 
        };

        using var client = new AmazonS3Client(credentials, config);

        // 3. Формируем запрос на загрузку файла
        using var newMemoryStream = new MemoryStream();
        await file.CopyToAsync(newMemoryStream); // Копируем файл из HTTP-запроса в оперативную память сервера

        var putRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = fileName, // Имя файла в облаке (например, "user_123.jpg")
            InputStream = newMemoryStream,
            ContentType = file.ContentType // Тип (image/jpeg, image/png)
        };

        // 4. Отправляем в MinIO!
        await client.PutObjectAsync(putRequest);

        // 5. Возвращаем готовую ссылку на файл
        return $"{endpoint}/{bucketName}/{fileName}";
    }
}