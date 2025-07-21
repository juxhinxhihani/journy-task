using Journey.Application.Abstractions.DbContext;
using Journey.Domain.Email;
using Journey.Domain.Users;
using Journey.Domain.Users.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Journey.Application.Users.Events;

internal sealed class UserCreatedDomainEventHandler(
    IEmailService _emailService,
    ILogger<UserCreatedDomainEventHandler> _logger) : INotificationHandler<UserCreatedDomainEvent>
{

    public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {

        var result = await _emailService.SendConfirmEmail(notification.email, notification.token);

        if (!result)
        {
            _logger.LogError($"Failed sending email for user: {notification.userId}");
            return;
        }

        _logger.LogDebug($"Email sent successfully for user: {notification.userId}");
    }
}
