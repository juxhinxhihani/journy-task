using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Reward.Application.Abstractions.DbContext;
using Reward.Domain.Journeys;
using Reward.Domain.OutboxMessages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Reward.Worker.Consumers;

public class JourneyCreatedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<JourneyCreatedConsumer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public JourneyCreatedConsumer(
        IServiceScopeFactory scopeFactory,
        ILogger<JourneyCreatedConsumer> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
            Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = configuration["RabbitMQ:UserName"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.QueueDeclare(queue: "JourneyCreated", durable: true, exclusive: false, autoDelete: false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DailyGoalAchievementConsumer started.");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var journeyCreated = JsonSerializer.Deserialize<JourneyCreated>(message);

                if (journeyCreated != null)
                {
                    await ProcessJourneyCreated(journeyCreated);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing JourneyCreated message");
                _channel.BasicNack(ea.DeliveryTag, false, true); // Requeue on error
            }
        };

        _channel.BasicConsume(queue: "JourneyCreated", autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task ProcessJourneyCreated(JourneyCreated message)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var userId = message.UserId;
        var journeyId = message.JourneyId;
        var journeyDistance = message.Distance;
        var journeyDate = message.CreatedAt.Date;

        _logger.LogInformation("Processing journey {JourneyId} with distance {Distance}km", journeyId, journeyDistance);

        // Save journey to local database first
        var existingJourney = await dbContext.Journeys.FirstOrDefaultAsync(j => j.Id == journeyId);
        if (existingJourney == null)
        {
            var journey = new Journey
            {
                Id = journeyId,
                UserId = userId,
                RouteDistanceKm = journeyDistance,
                CreatedOnUtc = message.CreatedAt,
                IsDailyGoalAchieved = false
            };
            
            dbContext.Journeys.Add(journey);
            await dbContext.SaveChangesAsync();
        }

        var dailyGoalAlreadyAchieved = await dbContext.Journeys
            .AnyAsync(j => j.UserId == userId &&
                           j.CreatedOnUtc.Date == journeyDate &&
                           j.IsDailyGoalAchieved);

        if (dailyGoalAlreadyAchieved)
        {
            _logger.LogInformation("User {UserId} already achieved daily goal on {Date}", userId, journeyDate);
            return;
        }

        var totalDailyDistance = await dbContext.Journeys
            .Where(j => j.UserId == userId && j.CreatedOnUtc.Date == journeyDate)
            .SumAsync(j => j.RouteDistanceKm);

        _logger.LogInformation("User {UserId} total daily distance: {Distance}km", userId, totalDailyDistance);

        if (totalDailyDistance >= 20.0m && (totalDailyDistance - journeyDistance) < 20.0m)
        {
            _logger.LogInformation("User {UserId} achieved daily goal! Total: {TotalDistance}km", userId, totalDailyDistance);

            var triggeringJourney = await dbContext.Journeys
                .FirstOrDefaultAsync(j => j.Id == journeyId);

            if (triggeringJourney != null)
            {
                triggeringJourney.IsDailyGoalAchieved = true;
                dbContext.Journeys.Update(triggeringJourney);

                var dailyGoalEvent = new
                {
                    UserId = userId,
                    JourneyId = journeyId,
                    TotalDistance = totalDailyDistance,
                    AchievedOn = DateTime.UtcNow
                };

                string payload = JsonSerializer.Serialize(dailyGoalEvent);

                var outboxMessage = OutboxMessage.Create("DailyGoalAchieved", payload);
                dbContext.OutboxMessages.Add(outboxMessage);

                await dbContext.SaveChangesAsync();

                _logger.LogInformation("Daily goal achievement recorded for user {UserId}", userId);
            }
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}