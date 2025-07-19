using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.Journys.ShareJourney;

public class ShareJourneyCommandHandler(IApplicationDbContext _context) : ICommandHandler<ShareJourneyCommand>
{
    public async Task<Result> Handle(ShareJourneyCommand request, CancellationToken cancellationToken)
    {
        var journey = await _context.Journeys
            .Include(j => j.SharedWithUsers)
            .FirstOrDefaultAsync(j => j.Id == request.Id);

        if (journey is null)
            return Result.Failure("Journey can not be found.");

        var existingShares = journey.SharedWithUsers.Select(js => js.UserId).ToHashSet();

        foreach (var userId in request.Users)
        {
            if (!existingShares.Contains(userId))
            {
                journey.ShareWithUser(userId);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}