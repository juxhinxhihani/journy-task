using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Application.Abstractions.Pagination;
using Journey.Application.DTOs.Response;
using Journey.Domain.Abstractions;

namespace Journey.Application.Journys.GetMonthlyRouteJourney;

public class GetMonthlyRouteJourneyQueryHandler(
    IApplicationDbContext _context)
    : IQueryHandler<GetMonthlyRouteJourneyQuery, PaginatedList<MonthlyRouteDistanceResponse>>
{
    public async Task<Result<PaginatedList<MonthlyRouteDistanceResponse>>> Handle(GetMonthlyRouteJourneyQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Journeys
            .Where(j => !j.IsDeleted)
            .AsQueryable();

        var currentDate = DateTime.UtcNow;

        int year = request.Year ?? currentDate.Year;
        int month = request.Month ?? currentDate.Month;

        query = query.Where(j => j.StartTime.Year == year && j.StartTime.Month == month);

        var groupedResult = query
            .GroupBy(j => new { j.UserId, j.User.FirstName, j.User.LastName })
            .Select(g => new MonthlyRouteDistanceResponse
            {
                UserId = g.Key.UserId,
                FullName = g.Key.FirstName + " " + g.Key.LastName,
                TotalDistanceKm = g.Sum(j => j.RouteDistanceKm)
            });


        //sorting
        groupedResult = request.ColumnName.ToLower() switch
        {
            "userid" => request.IsDescending
                ? groupedResult.OrderByDescending(g => g.UserId)
                : groupedResult.OrderBy(g => g.UserId),
            "totaldistancekm" => request.IsDescending
                ? groupedResult.OrderByDescending(g => g.TotalDistanceKm)
                : groupedResult.OrderBy(g => g.TotalDistanceKm),
            _ => groupedResult.OrderByDescending(g => g.TotalDistanceKm)
        };

        // Apply pagination
        var result = await groupedResult.PaginatedListAsync(request.PageNumber, request.PageSize);

        return Result.Success(result);
    }
}