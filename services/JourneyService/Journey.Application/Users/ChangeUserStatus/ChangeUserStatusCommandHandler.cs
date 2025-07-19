using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Domain.Abstractions;
using Journey.Domain.Users;
using Microsoft.Identity.Client;

namespace Journey.Application.Users.ChangeUserStatus;

public class ChangeUserStatusCommandHandler(
    IApplicationDbContext _context,
    IUserRepository _userRepository) 
    : ICommandHandler<ChangeUserStatusCommand>
{
    public async Task<Result> Handle(ChangeUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.Get(request.UserId);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        if (!Enum.TryParse<UserStatus>(request.Status, true, out var newStatus))
            return Result.Failure(Error.Validation("User.Status.Invalid", "Invalid user status."));

        if (user.Status == newStatus)
            return Result.Success();
        var oldStatus = user.Status;
        user.Status = newStatus;

        // _context.AuditLogs.Add(new AuditLog
        // {
        //     EntityName = nameof(User),
        //     EntityId = user.Id.ToString(),
        //     Action = "UserStatusChanged",
        //     OldValue = oldStatus.ToString(),
        //     NewValue = newStatus.ToString(),
        //     Timestamp = DateTime.UtcNow,
        //     PerformedBy = "admin" // or get from _loggedUser
        // });

        user.ChangeStatus(user.Id, user.Email ,oldStatus, newStatus);
        _userRepository.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();    }
}