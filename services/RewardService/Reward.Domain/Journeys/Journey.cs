
using System.ComponentModel.DataAnnotations.Schema;

using Reward.Domain.Abstractions;

namespace Reward.Domain.Journeys;

public class Journey : BaseAuditableEntity<Guid>
{
    public Journey() { }
    public Journey(Guid id) : base(id) { }
    public Journey(Guid userId, string startLocation, DateTime startTime, string arrivalLocation, DateTime arrivalTime, string transportationType, decimal routeDistanceKm)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        StartLocation = startLocation;
        StartTime = startTime;
        ArrivalLocation = arrivalLocation;
        ArrivalTime = arrivalTime;
        TransportationType = transportationType;
        RouteDistanceKm = routeDistanceKm;
    }
    
    public Guid UserId { get; set; }
    
    public string StartLocation { get; set; }
    public DateTime StartTime { get; set; }

    public string ArrivalLocation { get; set; }
    public DateTime ArrivalTime { get; set; }

    public string TransportationType { get; set; }
    public decimal RouteDistanceKm { get; set; }

    public bool IsDailyGoalAchieved { get; set; }

    public string? PublicLink { get; set; }
    public bool IsPublicLinkRevoked { get; set; }
    
    public bool IsDeleted { get; set; } 
}