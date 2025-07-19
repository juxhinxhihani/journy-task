using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Application.Abstractions.Pagination;
using Journey.Application.DTOs.Response;
using Journey.Domain.Abstractions;
using Journey.Domain.Abstractions.Interface;
using Journey.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.Journys.GetSharedJourneys;

public sealed class GetSharedJourneysQueryHandler(
    IActualUser _loggedUser,
    IApplicationDbContext _context)
    : IQueryHandler<GetSharedJourneysQuery, PaginatedList<JourneyShareResponse>>
{
    public async Task<Result<PaginatedList<JourneyShareResponse>>> Handle(GetSharedJourneysQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_loggedUser.Id, out var userId))
        {
            return Result.Failure<PaginatedList<JourneyShareResponse>>(UserErrors.NotLoggedIn);
        }

        var query = _context.JourneyShare
            .Include(js => js.Journey)
            .ThenInclude(j => j.User)
            .Where(js => js.UserId == userId && !js.Journey.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.SearchKey))
        {
            query = query.Where(js =>
                js.Journey.StartLocation.Contains(request.SearchKey) ||
                js.Journey.ArrivalLocation.Contains(request.SearchKey) ||
                js.Journey.TransportationType.Contains(request.SearchKey));
        }

        var result = await query
            .SortAndOrderBy(request.ColumnName, request.IsDescending)
            .Select(js => new JourneyShareResponse
            {
                JourneyId = js.Journey.Id,
                StartLocation = js.Journey.StartLocation,
                StartTime = js.Journey.StartTime,
                ArrivalLocation = js.Journey.ArrivalLocation,
                ArrivalTime = js.Journey.ArrivalTime,
                TransportationType = js.Journey.TransportationType,
                RouteDistanceKm = js.Journey.RouteDistanceKm,
                IsDailyGoalAchieved = js.Journey.IsDailyGoalAchieved,
                SharedByEmail = js.Journey.User.Email,
                SharedAt = js.CreatedOnUtc
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return Result.Success(result);
    }
}