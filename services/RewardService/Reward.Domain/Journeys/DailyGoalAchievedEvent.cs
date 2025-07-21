namespace Reward.Domain.Journeys;

public class DailyGoalAchievedEvent
{
    public Guid UserId { get; set; }
    public Guid JourneyId { get; set; }
    public decimal TotalDistance { get; set; }
    public DateTime AchievedOn { get; set; }
}