namespace NigerianPrimarySchool.Domain.Common;

/// <summary>
/// Base class for all domain entities that require auditing.
/// </summary>
public abstract class BaseAuditableEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}
