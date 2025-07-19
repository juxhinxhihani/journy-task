using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Application.Abstractions.Pagination;
using Journey.Application.DTOs.Response;
using Journey.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.Journys.GetJourneys;

public class GetJourneysQueryHandler(
    IApplicationDbContext _context) 
    : IQueryHandler<GetJourneysQuery, PaginatedList<AllJourneysResponse>>
{
    public async Task<Result<PaginatedList<AllJourneysResponse>>> Handle(GetJourneysQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Journeys
            .Include(j => j.User)
            .AsQueryable();

        if (request.UserId.HasValue)
            query = query.Where(j => j.UserId == request.UserId.Value);

        if (!string.IsNullOrWhiteSpace(request.TransportationType))
            query = query.Where(j => j.TransportationType.Contains(request.TransportationType));

        if (request.StartDateFrom.HasValue)
            query = query.Where(j => j.StartTime >= request.StartDateFrom.Value);

        if (request.StartDateTo.HasValue)
            query = query.Where(j => j.StartTime <= request.StartDateTo.Value);

        if (request.ArrivalDateFrom.HasValue)
            query = query.Where(j => j.ArrivalTime >= request.ArrivalDateFrom.Value);

        if (request.ArrivalDateTo.HasValue)
            query = query.Where(j => j.ArrivalTime <= request.ArrivalDateTo.Value);

        query = query.SortAndOrderBy(request.ColumnName, request.IsDescending);

        var result = await query.Select(j => new AllJourneysResponse
        {
            JourneyId = j.Id,
            StartLocation = j.StartLocation,
            ArrivalLocation = j.ArrivalLocation,
            StartTime = j.StartTime,
            ArrivalTime = j.ArrivalTime,
            RouteDistanceKm = j.RouteDistanceKm,
            TransportationType = j.TransportationType,
            IsPublic = !j.IsPublicLinkRevoked,
            PublicLink = j.PublicLink,
            IsDailyGoalAchieved = j.IsDailyGoalAchieved,
            CreatedAt = j.CreatedOnUtc,
            UserId = j.User.Id,
            FirstName = j.User.FirstName,
            LastName = j.User.LastName
        }).PaginatedListAsync(request.PageNumber, request.PageSize);

        return Result.Success(result);
    }
}