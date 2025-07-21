using System.Text;
using Journey.Domain.Abstractions.Interface;
using Journey.Infrastructure;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace Journey.API.Workers;

public class OutboxPublisherWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxPublisherWorker> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly int _intervalMs;

    public OutboxPublisherWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxPublisherWorker> logger,
        IConfiguration config)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _intervalMs = config.GetValue("Outbox:PollingIntervalMs", 1000);

        var factory = new ConnectionFactory
        {
            HostName = config["RabbitMQ:HostName"] ?? "localhost",
            Port = int.Parse(config["RabbitMQ:Port"] ?? "5672"),
            UserName = config["RabbitMQ:UserName"] ?? "guest",
            Password = config["RabbitMQ:Password"] ?? "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxPublisherWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var messages = await db.OutboxMessages
                    .Where(m => !m.Processed && m.Type == "JourneyCreated")
                    .OrderBy(m => m.OccurredOnUtc)
                    .Take(10)
                    .ToListAsync(stoppingToken);

                foreach (var msg in messages)
                {
                    try
                    {
                        await PublishMessage(msg);
                        msg.MarkAsProcessed();
                        db.OutboxMessages.Update(msg);

                        _logger.LogInformation("Published JourneyCreated message {MessageId} for processing", 
                            msg.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to publish JourneyCreated message {MessageId}", msg.Id);
                    }
                }

                if (messages.Any())
                {
                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OutboxPublisherWorker execution");
            }

            await Task.Delay(_intervalMs, stoppingToken);
        }

        _logger.LogInformation("OutboxPublisherWorker stopped.");
    }

    private async Task PublishMessage(Domain.OutboxMessages.OutboxMessage message)
    {
        _channel.ExchangeDeclare(exchange: "journey.events", type: ExchangeType.Direct, durable: true);
        
        _channel.QueueDeclare(queue: message.Type, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(queue: message.Type, exchange: "journey.events", routingKey: message.Type);

        var body = Encoding.UTF8.GetBytes(message.Payload);
        _channel.BasicPublish(exchange: "journey.events", routingKey: message.Type, basicProperties: null, body: body);

        await Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
