using Journey.Domain.Email;
using Journey.Domain.Users.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Journey.Application.Users.Events;

public class DailyGoalAchievedDomainHandler(
    IEmailService _emailService,
    ILogger<DailyGoalAchievedDomainHandler> _logger) : INotificationHandler<DailyGoalAchievedDomain>
{
    public async Task Handle(DailyGoalAchievedDomain notification, CancellationToken cancellationToken)
    {
        var emailResult = await _emailService.SendDailyGoalAchive(
            notification.email,
            notification.totalDistance,
            notification.messageAchievedOn);
        if (!emailResult)
        {
            _logger.LogError($"Failed sending daily goal achieved email for user: {notification.email}");
            return;
        }

        _logger.LogDebug($"Daily goal achieved email sent successfully for user: {notification.email}");
    }
}