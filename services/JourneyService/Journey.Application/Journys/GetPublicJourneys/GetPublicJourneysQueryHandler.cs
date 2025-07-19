using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Application.Abstractions.Pagination;
using Journey.Application.DTOs.Response;
using Journey.Domain.Abstractions;
using Journey.Domain.Journeys.Interface;

namespace Journey.Application.Journys.GetPublicJourneys;

public sealed class GetPublicJourneysQueryHandler(
    IApplicationDbContext _context) 
    : IQueryHandler<GetPublicJourneysQuery, PaginatedList<JourneysResponse>>
{
    public async Task<Result<PaginatedList<JourneysResponse>>> Handle(GetPublicJourneysQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Journeys
            .Where(j => j.PublicLink != null && !j.IsPublicLinkRevoked && !j.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.SearchKey))
        {
            query = query.Where(j =>
                j.StartLocation.Contains(request.SearchKey) ||
                j.ArrivalLocation.Contains(request.SearchKey) ||
                j.TransportationType.Contains(request.SearchKey));
        }

        var result = await query.SortAndOrderBy(request.ColumnName, request.IsDescending)
            .Select(j => new JourneysResponse
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
                CreatedAt = j.CreatedOnUtc
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize);
        
        return result;
    }
}