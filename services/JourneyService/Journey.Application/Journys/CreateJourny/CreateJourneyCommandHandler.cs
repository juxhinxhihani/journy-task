using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Application.DTOs;
using Journey.Domain.Abstractions;
using Journey.Domain.Abstractions.Interface;
using Journey.Domain.Journeys;
using Journey.Domain.OutboxMessages;
using Journey.Domain.OutboxMessages.Interface;

namespace Journey.Application.Journys.AddJourny;

internal sealed class CreateJourneyCommandHandler(
      IApplicationDbContext _context,
      IActualUser _loggedInUser,
      IOutboxMessageRepository _outboxMessageRepository)
    : ICommandHandler<CreateJourneyCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateJourneyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(_loggedInUser.Id, out var userId))
            {
                return Result.Failure<Guid>(Error.Forbidden("CreateJourney.","Invalid User ID"));
            }

            var journey = Domain.Journeys.Journey.Create(
                userId,
                request.dto.StartLocation,
                request.dto.StartTime,
                request.dto.ArrivalLocation,
                request.dto.ArrivalTime,
                request.dto.TransportationType,
                request.dto.RouteDistanceKm
            );

            _context.Journeys.Add(journey);
            await _context.SaveChangesAsync(cancellationToken);

            var journeyCreatedEvent = new
            {
                UserId = userId,
                JourneyId = journey.Id,
                Distance = (decimal)journey.RouteDistanceKm,
                CreatedAt = DateTime.UtcNow
            };

            var outboxMessage = OutboxMessage.Create("JourneyCreated", journeyCreatedEvent);
            _context.OutboxMessages.Add(outboxMessage);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(journey.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>(Error.Failure("CreateJourney.Error",$"An error occurred: {ex.Message}"));
        }
    }
}