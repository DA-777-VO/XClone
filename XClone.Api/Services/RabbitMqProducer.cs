using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace XClone.Api.Services;

public class RabbitMqProducer : IMessageProducer
{
    public void SendMessage<T>(T message, string queueName)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: queueName, 
            durable: true, 
            exclusive: false, 
            autoDelete: false, 
            arguments: null
        );
        
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        
        channel.BasicPublish(exchange: "",
            routingKey: queueName, 
            basicProperties: null, 
            body: body);
    }
}