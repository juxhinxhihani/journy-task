using Journey.Domain.Abstractions.Interface;

namespace Journey.Domain.Abstractions;

public abstract class BaseAuditableEntity<T> : Entity<T>, IBaseAuditableEntity
{
    protected BaseAuditableEntity(T id) : base(id)
    {
    }

    protected BaseAuditableEntity() { }

    public DateTime CreatedOnUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime LastModifiedOnUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}