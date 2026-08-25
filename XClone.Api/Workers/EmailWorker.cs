using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using XClone.Api.Events; 

namespace XClone.Api.Workers;

public class EmailWorker : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;
    private const string QueueName = "emails_queue";
    
    public EmailWorker()
    {
        var factory = new ConnectionFactory { HostName = "localhost", 
            DispatchConsumersAsync = true  };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.QueueDeclare(queue: QueueName, 
            durable: true, 
            exclusive: false, 
            autoDelete: false, 
            arguments: null
        );
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            
            var userEvent = JsonSerializer.Deserialize<UserRegisteredEvent>(json);

            try
            {
                // imitation
                Console.WriteLine($"[EMAIL WORKER] Начинаю отправку письма для {userEvent.Email}...");
                await Task.Delay(3000, stoppingToken);
                Console.WriteLine($"[EMAIL WORKER] УСПЕХ! Письмо 'Добро пожаловать, {userEvent.Username}' отправлено.");
                
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL WORKER] Ошибка отправки: {ex.Message}");
                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };
        
        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }
    
    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}