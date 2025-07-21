namespace Reward.Domain.Journeys;

public class JourneyCreated
{
    public Guid UserId { get; set; }
    public Guid JourneyId { get; set; }
    public decimal Distance { get; set; }
    public DateTime CreatedAt { get; set; }

    public JourneyCreated() { }

    public JourneyCreated(Guid userId, Guid journeyId, decimal distance)
    {
        UserId = userId;
        JourneyId = journeyId;
        Distance = distance;
        CreatedAt = DateTime.UtcNow;
    }
}