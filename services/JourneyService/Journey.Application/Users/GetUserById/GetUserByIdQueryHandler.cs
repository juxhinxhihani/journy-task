using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Application.DTOs.Response;
using Journey.Domain.Abstractions;
using Journey.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.Users.GetUserById;

public class GetUserByIdQueryHandler(
    IApplicationDbContext _context,
    IUserRepository _userRepository) : IQueryHandler<GetUserByIdQuery, UserByIdResponse>
{
    public async Task<Result<UserByIdResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Where(u => u.Id == request.Id)
            .Include(u => u.JournyShares)
            .ThenInclude(js => js.Journey)
            .ThenInclude(j => j.User)
            .Include(u => u.Journys)
            .FirstOrDefaultAsync();

        if (user != null)
        {
            user.JournyShares = user.JournyShares
                .Where(js => js.Journey != null && !js.Journey.IsDeleted)
                .ToList();

            user.Journys = user.Journys
                .Where(j => !j.IsDeleted)
                .ToList();
        }
        
        if (user is null)
        {
            return Result.Failure<UserByIdResponse>(UserErrors.NotFound);
        }
        return new UserByIdResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            Role = user.Role.ToString(),
            IsDeleted = user.IsDeleted,
            Status = user.Status.ToString(),
            MyJourneys = user.Journys.Select(j => new JourneysResponse
            {
                JourneyId = j.Id,
                StartLocation = j.StartLocation,
                StartTime = j.StartTime,
                ArrivalLocation = j.ArrivalLocation,
                ArrivalTime = j.ArrivalTime,
                RouteDistanceKm = j.RouteDistanceKm,
                TransportationType = j.TransportationType,
                IsDailyGoalAchieved = j.IsDailyGoalAchieved
            }).ToList(),
            JourneySharedWithMe = user.JournyShares.Select(j => new JourneyShareResponse
            {
                JourneyId = j.Id,
                StartLocation = j.Journey.StartLocation,
                StartTime = j.Journey.StartTime,
                ArrivalLocation = j.Journey.ArrivalLocation,
                ArrivalTime = j.Journey.ArrivalTime,
                RouteDistanceKm = j.Journey.RouteDistanceKm,
                TransportationType = j.Journey.TransportationType,
                SharedAt = j.CreatedOnUtc,
                IsDailyGoalAchieved = j.Journey.IsDailyGoalAchieved,
                SharedByEmail = j.Journey.User.Email
            }).ToList()
        };
    }
}