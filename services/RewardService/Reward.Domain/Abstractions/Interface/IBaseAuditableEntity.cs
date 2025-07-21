namespace Reward.Domain.Abstractions.Interface;

public interface IBaseAuditableEntity
{
    public DateTime CreatedOnUtc { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime LastModifiedOnUtc { get; set; }

    public string? LastModifiedBy { get; set; }
}