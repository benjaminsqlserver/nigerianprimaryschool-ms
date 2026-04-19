using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NigerianPrimarySchool.Infrastructure.Identity;

namespace NigerianPrimarySchool.Infrastructure.Persistence;

/// <summary>Single EF Core DbContext. Additional domain DbSets added per module.</summary>
public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Rename default Identity tables
        builder.Entity<ApplicationUser>()  .ToTable("Users");
        builder.Entity<ApplicationRole>()  .ToTable("Roles");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>()   .ToTable("UserRoles");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>()  .ToTable("UserClaims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>()  .ToTable("UserLogins");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>()  .ToTable("RoleClaims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>()  .ToTable("UserTokens");

        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.LastName).HasDatabaseName("IX_Users_LastName");
        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.IsActive).HasDatabaseName("IX_Users_IsActive");
    }
}
