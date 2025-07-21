using System.Text;
using Journey.Domain.Abstractions.Interface;
using Journey.Domain.OutboxMessages;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using IModel = RabbitMQ.Client.IModel;

namespace Journey.Infrastructure.Messaging;

public class RabbitMQPublisher : IOutboxPublisher
{
    private readonly IModel _channel;
    private readonly IConnection _connection;

    public RabbitMQPublisher(IOptions<RabbitMQOptions> options)
    {
        var config = options.Value;

        var factory = new ConnectionFactory
        {
            HostName = config.HostName,
            Port = config.Port,
            UserName = config.UserName,
            Password = config.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public Task PublishAsync(OutboxMessage message)
    {
        _channel.QueueDeclare(queue: message.Type, durable: true, exclusive: false, autoDelete: false);

        var body = Encoding.UTF8.GetBytes(message.Payload);
        _channel.BasicPublish(exchange: "", routingKey: message.Type, basicProperties: null, body: body);

        return Task.CompletedTask;
    }

    public Task PublishAsync(string eventType, string payload)
    {
        _channel.QueueDeclare(queue: eventType, durable: true, exclusive: false, autoDelete: false);

        var body = Encoding.UTF8.GetBytes(payload);
        _channel.BasicPublish(exchange: "", routingKey: eventType, basicProperties: null, body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
