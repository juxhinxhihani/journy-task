using Journey.Domain.Email;
using Journey.Domain.Users.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Journey.Application.Users.Events;

public class UserStatusChangedDomainEventHandler(
    IEmailService _emailService,
    ILogger<UserStatusChangedDomainEventHandler> _logger
    )
    : INotificationHandler<UserStatusChangedDomainEvent>
{
    public async Task Handle(UserStatusChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var emailResult = await _emailService.SendStatusChange(
                                            notification.email, 
                                            notification.oldStatus.ToString(), 
                                            notification.newStatus.ToString());
        if (!emailResult)
        {
            _logger.LogError($"Failed sending activation email for user: {notification.email}");
            return;
        }

        _logger.LogDebug($"Activation email sent successfully for user: {notification.email}");
    }
}