namespace Journey.Domain.Abstractions.Interface;

public interface IEntity
{
    void ClearDomainEvents();
    IReadOnlyList<IDomainEvent> GetDomainEvents();
}