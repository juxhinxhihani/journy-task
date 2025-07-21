using System.ComponentModel.DataAnnotations.Schema;
using Journey.Domain.Abstractions;
using Journey.Domain.Events;
using Journey.Domain.Users;

namespace Journey.Domain.Journeys;

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
    
    [ForeignKey(nameof(User))]
    public Guid UserId { get; private set; }
    public User User { get; private set; } 
    
    public string StartLocation { get; private set; }
    public DateTime StartTime { get; private set; }

    public string ArrivalLocation { get; private set; }
    public DateTime ArrivalTime { get; private set; }

    public string TransportationType { get; private set; }
    public decimal RouteDistanceKm { get; private set; }

    public bool IsDailyGoalAchieved { get; private set; }

    public string? PublicLink { get; private set; }
    public bool IsPublicLinkRevoked { get; private set; }
    
    public bool IsDeleted { get; private set; } = false;
    
    public ICollection<JourneyShare> SharedWithUsers { get; private set; } = new List<JourneyShare>();

    public static Journey Create(Guid userId, string startLocation, DateTime startTime, 
                                    string arrivalLocation, DateTime arrivalTime, 
                                    string transportationType, decimal routeDistanceKm)
    {
        return new Journey(userId, startLocation, startTime, arrivalLocation, arrivalTime, transportationType, routeDistanceKm);
    }

    public void ShareWithUser(Guid userId)
    {
        if (SharedWithUsers.Any(s => s.UserId == userId)) 
            return;
        SharedWithUsers.Add(JourneyShare.Create(journeyId: Id, userId: userId));
    }

    public void RevokeShare(Guid userId)
    {
        var share = SharedWithUsers.FirstOrDefault(s => s.UserId == userId);
        if (share != null) SharedWithUsers.Remove(share);
    }

    public void GeneratePublicLink(string link)
    {
        PublicLink = link;
        IsPublicLinkRevoked = false;
    }
    public void Delete()
    {
        IsDeleted = true;
    }

    public void RevokePublicLink()
    {
        IsPublicLinkRevoked = true;
    }

    public void MarkDailyGoalAchieved()
    {
        IsDailyGoalAchieved = true;
    }
    public void MarkDailyGoalAchieved(decimal totalDailyDistance)
    {
        if (!IsDailyGoalAchieved)
        {
            IsDailyGoalAchieved = true;
            
            RaiseDomainEvent(new DailyGoalAchievedEvent
            {
                UserId = UserId,
                JourneyId = Id,
                TotalDistance = totalDailyDistance,
                AchievedOn = DateTime.UtcNow
            });
        }
    }
}