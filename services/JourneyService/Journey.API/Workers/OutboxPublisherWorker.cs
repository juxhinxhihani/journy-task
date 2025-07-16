using Journey.Domain.Abstractions.Interface;
using Journey.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Journey.API.Workers;

public class OutboxPublisherWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxPublisherWorker> logger,
    IConfiguration config)
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<OutboxPublisherWorker> _logger = logger;
    private readonly int _intervalMs = config.GetValue("Outbox:PollingIntervalMs", 1000);


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxPublisherWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var publisher = scope.ServiceProvider.GetRequiredService<IOutboxPublisher>();

            var messages = await db.OutboxMessages
                .Where(m => !m.Processed)
                .Take(10)
                .ToListAsync(stoppingToken);

            foreach (var msg in messages)
            {
                try
                {
                    await publisher.PublishAsync(msg.Type, msg.Payload);
                    msg.MarkAsProcessed();
                    _logger.LogInformation("Published and marked processed: {MessageId}", msg.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish message: {MessageId}", msg.Id);
                }
            }

            await db.SaveChangesAsync(stoppingToken);
            await Task.Delay(_intervalMs, stoppingToken);
        }
        _logger.LogInformation("OutboxPublisherWorker stopped.");
    }
}
    