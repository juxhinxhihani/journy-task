using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Journey.Application.Abstractions.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Journey.API.Consumers;

public class DailyGoalAchievedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DailyGoalAchievedConsumer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public DailyGoalAchievedConsumer(
        IServiceScopeFactory scopeFactory,
        ILogger<DailyGoalAchievedConsumer> logger,
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
        
        _channel.QueueDeclare(queue: "DailyGoalAchieved", durable: true, exclusive: false, autoDelete: false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DailyGoalAchievedConsumer started");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var dailyGoalAchieved = JsonSerializer.Deserialize<DailyGoalAchievedEvent>(message);

                if (dailyGoalAchieved != null)
                {
                    await ProcessDailyGoalAchieved(dailyGoalAchieved);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing DailyGoalAchieved message");
                _channel.BasicNack(ea.DeliveryTag, false, true); // Requeue on error
            }
        };

        _channel.BasicConsume(queue: "DailyGoalAchieved", autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task ProcessDailyGoalAchieved(DailyGoalAchievedEvent message)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        _logger.LogInformation("Processing daily goal achievement for user {UserId}, journey {JourneyId}, total distance: {TotalDistance}km", 
            message.UserId, message.JourneyId, message.TotalDistance);

        try
        {
            var triggeringJourney = await dbContext.Journeys.FirstOrDefaultAsync(j => j.Id == message.JourneyId);

            if (triggeringJourney != null && !triggeringJourney.IsDailyGoalAchieved)
            {
                triggeringJourney.MarkDailyGoalAchieved();
                dbContext.Journeys.Update(triggeringJourney);
                await dbContext.SaveChangesAsync();

                _logger.LogInformation("Journey {JourneyId} marked as daily goal achieved for user {UserId}", 
                    message.JourneyId, message.UserId);
            }
            else if (triggeringJourney == null)
            {
                _logger.LogWarning("Journey {JourneyId} not found for daily goal achievement", message.JourneyId);
            }
            else
            {
                _logger.LogInformation("Journey {JourneyId} already marked as daily goal achieved", message.JourneyId);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process daily goal achievement for user {UserId}", message.UserId);
            throw; 
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}

public class DailyGoalAchievedEvent
{
    public Guid UserId { get; set; }
    public Guid JourneyId { get; set; }
    public decimal TotalDistance { get; set; }
    public DateTime AchievedOn { get; set; }
}
