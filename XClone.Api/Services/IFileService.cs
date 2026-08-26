namespace XClone.Api.Services;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file, string fileName);
}