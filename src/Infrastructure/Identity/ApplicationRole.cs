using Microsoft.AspNetCore.Identity;

namespace NigerianPrimarySchool.Infrastructure.Identity;

/// <summary>Extended Identity role with description.</summary>
public class ApplicationRole : IdentityRole
{
    public string?   Description { get; set; }
    public DateTime  CreatedAt   { get; set; } = DateTime.UtcNow;

    public ApplicationRole() { }
    public ApplicationRole(string roleName, string? description = null)
        : base(roleName) => Description = description;
}
