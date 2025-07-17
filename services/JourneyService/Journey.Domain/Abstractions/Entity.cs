using Journey.Domain.Abstractions.Interface;

namespace Journey.Domain.Abstractions;

public abstract class Entity<T> : IEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    protected Entity(T id)
    {
        Id = id;
    }

    protected Entity()
    {
    }

    public T Id { get; init; }

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
