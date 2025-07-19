using System.Net;
using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Application.DTOs.Response;
using Journey.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.Journys.GetJourneyByPublicLinkToken;

public class GetPublicJourneysByTokenQueryHandler(
    IApplicationDbContext _context) 
    : IQueryHandler<GetPublicJourneysByTokenQuery, JourneysResponse>
{
    public async Task<Result<JourneysResponse>> Handle(GetPublicJourneysByTokenQuery request, CancellationToken cancellationToken)
    { var journey = await _context.Journeys
            .AsNoTracking()
            .FirstOrDefaultAsync(j =>
                    j.PublicLink != null &&
                    j.PublicLink.EndsWith(request.Token), 
                    cancellationToken);

        if (journey is null)
            return Result.Failure<JourneysResponse>(Error.NotFound("Journey.NotFound", "Journey can not be found."));

        if (journey.IsPublicLinkRevoked)
        {
            return Result.Failure<JourneysResponse>(Error.Gone(HttpStatusCode.Gone));
        }
        var response = new JourneysResponse
        {
            JourneyId = journey.Id,
            StartLocation = journey.StartLocation,
            StartTime = journey.StartTime,
            ArrivalLocation = journey.ArrivalLocation,
            ArrivalTime = journey.ArrivalTime,
            TransportationType = journey.TransportationType,
            RouteDistanceKm = journey.RouteDistanceKm,
            IsDailyGoalAchieved = journey.IsDailyGoalAchieved,
            IsPublic = !journey.IsPublicLinkRevoked,
            PublicLink = journey.PublicLink,
            CreatedAt = journey.CreatedOnUtc
        };

        return Result.Success(response);
    }
}