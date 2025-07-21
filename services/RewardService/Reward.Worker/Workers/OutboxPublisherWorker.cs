using System.Text;
using RabbitMQ.Client;
using Reward.Application.Abstractions.DbContext;
using Reward.Domain.OutboxMessages;
using Microsoft.EntityFrameworkCore;

namespace Reward.Worker.Workers;

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
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _intervalMs = configuration.GetValue("Outbox:IntervalInSeconds", 10) * 1000;

        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
            Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = configuration["RabbitMQ:UserName"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest"
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
                var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                var unprocessedMessages = await dbContext.OutboxMessages
                    .Where(m => !m.Processed && m.Type == "DailyGoalAchieved")
                    .OrderBy(m => m.OccurredOnUtc)
                    .Take(10) 
                    .ToListAsync(stoppingToken);

                foreach (var message in unprocessedMessages)
                {
                    try
                    {
                        await PublishMessage(message);
                        message.MarkAsProcessed();
                        dbContext.OutboxMessages.Update(message);

                        _logger.LogInformation("Published DailyGoalAchieved message {MessageId} for processing", 
                            message.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to publish DailyGoalAchieved message {MessageId}", message.Id);
                    }
                }

                if (unprocessedMessages.Any())
                {
                    await dbContext.SaveChangesAsync(stoppingToken);
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

    private async Task PublishMessage(OutboxMessage message)
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
