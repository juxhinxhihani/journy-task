namespace Reward.Domain.Rewards;

public class Reward
{
    public Guid Id { get; private set; }

    public Guid JourneyId { get; private set; }

    public string UserId { get; private set; }

    public string Type { get; private set; }

    public int Points { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string? Description { get; private set; }

    private Reward() { }

    private Reward(Guid journeyId, string userId, string type, int points, string? description)
    {
        Id = Guid.NewGuid();
        JourneyId = journeyId;
        UserId = userId;
        Type = type;
        Points = points;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    public static Reward Create(Guid journeyId, string userId, string type, int points, string? description = null)
    {
        return new Reward(journeyId, userId, type, points, description);
    }
}
