namespace Journey.Application.DTOs.Response;

public class UserJourneysResponse
{
    public Guid JourneyId { get; set; }

    public string StartLocation { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }

    public string ArrivalLocation { get; set; } = string.Empty;
    public DateTime ArrivalTime { get; set; }

    public string TransportationType { get; set; } = string.Empty;
    public decimal RouteDistanceKm { get; set; }

    public bool IsDailyGoalAchieved { get; set; }

    public bool IsPublic { get; set; }
    public string? PublicLink { get; set; }

    public DateTime CreatedOnUtc { get; set; }
}