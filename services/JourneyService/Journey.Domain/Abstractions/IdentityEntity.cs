using Journey.Domain.Abstractions.Interface;
using Microsoft.AspNetCore.Identity;

namespace Journey.Domain.Abstractions;

public abstract class IdentityEntity : IdentityUser<Guid>, IEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    protected IdentityEntity()
    {
        Id = Guid.NewGuid();
    }

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
    }}