using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Application.Abstractions.Pagination;
using Journey.Application.DTOs.Response;
using Journey.Domain.Abstractions;
using Journey.Domain.Journeys.Interface;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.Journys.GetUserJourneys;

public sealed class GetJourneysByUserQueryHandler(
    IJourneyRepository _journeyRepository,
    IApplicationDbContext _context) 
    : IQueryHandler<GetJourneysByUserQuery, PaginatedList<UserJourneysResponse>>
{
    public async Task<Result<PaginatedList<UserJourneysResponse>>> Handle(GetJourneysByUserQuery request,  CancellationToken cancellationToken)
    {
        var journeys = _context.Journeys.Where(j => !j.IsDeleted);
        if (request.UserId.HasValue)
        {
            journeys = journeys.Where(j => j.UserId == request.UserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchKey))
        {
            journeys = journeys.Where(j =>
                j.StartLocation.Contains(request.SearchKey) ||
                j.ArrivalLocation.Contains(request.SearchKey) ||
                j.RouteDistanceKm.ToString().Contains(request.SearchKey) ||
                j.ArrivalLocation.Contains(request.SearchKey) ||
                j.TransportationType.Contains(request.SearchKey));
        }

        var result = await journeys.SortAndOrderBy(request.ColumnName, request.IsDescending)
            .Select(j => new UserJourneysResponse
            {
                JourneyId = j.Id,
                StartLocation = j.StartLocation,
                StartTime = j.StartTime,
                ArrivalLocation = j.ArrivalLocation,
                ArrivalTime = j.ArrivalTime,
                TransportationType = j.TransportationType,
                RouteDistanceKm = j.RouteDistanceKm,
                IsDailyGoalAchieved = j.IsDailyGoalAchieved,
                IsPublic = !j.IsPublicLinkRevoked,
                PublicLink = j.PublicLink,
                CreatedOnUtc = j.CreatedOnUtc
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize);
        
        return result;
    }
}