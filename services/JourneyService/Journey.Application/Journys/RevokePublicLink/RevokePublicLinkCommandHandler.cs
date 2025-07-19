using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Domain.Abstractions;
using Journey.Domain.Abstractions.Interface;
using Journey.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.Journys.RevokePublicLink;

public sealed class RevokeJourneyPublicLinkCommandHandler(
    IApplicationDbContext _context,
    IActualUser _loggedUser)
    : ICommandHandler<RevokeJourneyPublicLinkCommand>
{
    public async Task<Result> Handle(RevokeJourneyPublicLinkCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_loggedUser.Id, out var userId))
        {
            return Result.Failure(UserErrors.NotLoggedIn);
        }

        var journey = await _context.Journeys
            .FirstOrDefaultAsync(j => j.Id == request.JourneyId && j.UserId == userId, cancellationToken);

        if (journey is null)
        {
            return Result.Failure(Error.NotFound("Journey.NotFound", "Journey not found or not owned by user."));
        }
        if (journey.PublicLink is null)
        {
            return Result.Failure(Error.NotFound("Journey.PublicLinkNotFound", "Journey does not have a public link to revoke."));
        }
        if (journey.IsPublicLinkRevoked) 
        {
            return Result.Failure(Error.Conflict("Journey.PublicLinkAlreadyRevoked", "Public link is already revoked."));
        }
        
        //revoke
        journey.RevokePublicLink();
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}