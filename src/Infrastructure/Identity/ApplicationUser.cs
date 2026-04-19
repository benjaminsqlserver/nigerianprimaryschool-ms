using Microsoft.AspNetCore.Identity;

namespace NigerianPrimarySchool.Infrastructure.Identity;

/// <summary>Extended Identity user for Nigerian Primary School.</summary>
public class ApplicationUser : IdentityUser
{
    public string  FirstName  { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string  LastName   { get; set; } = string.Empty;

    /// <summary>Computed — not persisted to DB.</summary>
    public string FullName =>
        string.IsNullOrWhiteSpace(MiddleName)
            ? $"{FirstName} {LastName}"
            : $"{FirstName} {MiddleName} {LastName}";

    public DateTime? DateOfBirth      { get; set; }
    public string?   Gender           { get; set; }
    public string?   Address          { get; set; }
    public string?   ProfilePictureUrl { get; set; }

    /// <summary>Soft-disable flag.</summary>
    public bool     IsActive       { get; set; } = true;
    public DateTime CreatedAt      { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt   { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
