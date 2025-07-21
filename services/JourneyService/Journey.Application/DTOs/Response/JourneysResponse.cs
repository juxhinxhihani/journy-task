namespace Journey.Application.DTOs.Response;

public class JourneysResponse
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

    public DateTime CreatedAt { get; set; }
}

public class JourneyShareResponse : JourneysResponse
{
    public string SharedByEmail { get; set; } 

    public DateTime SharedAt { get; set; }
}

public class AllJourneysResponse : JourneysResponse
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}