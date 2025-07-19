namespace Journey.Application.DTOs.Response;

public class JourneysResponse
{
    public Guid JourneyId { get; set; }

    public string StartLocation { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }

    public string ArrivalLocation { get; set; } = string.Empty;
    public DateTime ArrivalTime { get; set; }

    public string TransportationType { get; set; } = string.Empty;
    public double RouteDistanceKm { get; set; }

    public bool IsDailyGoalAchieved { get; set; }

    public bool IsPublic { get; set; }
    public string? PublicLink { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class JourneyShareResponse : JourneysResponse
{
    public string SharedByEmail { get; set; } 

    public DateTime SharedAt { get; set; }
}