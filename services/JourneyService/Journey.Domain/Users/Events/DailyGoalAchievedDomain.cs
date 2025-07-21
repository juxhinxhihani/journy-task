using Journey.Domain.Abstractions.Interface;

namespace Journey.Domain.Users.Events;

public record class DailyGoalAchievedDomain(string email, decimal totalDistance, DateTime messageAchievedOn) : IDomainEvent;
