using Journey.Domain.Abstractions.Interface;

namespace Journey.Domain.Events;

public sealed class DailyGoalAchievedEvent : IDomainEvent
{
    public Guid UserId { get; init; }
    public Guid JourneyId { get; init; }
    public decimal TotalDistance { get; init; }
    public DateTime AchievedOn { get; init; }
}
