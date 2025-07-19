using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Domain.Abstractions;
using Journey.Domain.Journeys.Interface;

namespace Journey.Application.Journys.DeleteJourney;

public class DeleteJourneyCommandHandler(
    IApplicationDbContext _context,
    IJourneyRepository _journeyRepository) :
    ICommandHandler<DeleteJourneyCommand>
{
    public async Task<Result> Handle(DeleteJourneyCommand request, CancellationToken cancellationToken)
    {
        var journey = await _journeyRepository.Get(request.id);

        if (journey == null || journey.IsDeleted)
        {
            return Result.Failure("Journey not found or already deleted.");
        }

        journey.Delete();
        _journeyRepository.Update(journey);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}