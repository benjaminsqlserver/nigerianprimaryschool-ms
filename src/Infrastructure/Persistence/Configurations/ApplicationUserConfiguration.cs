using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NigerianPrimarySchool.Infrastructure.Identity;

namespace NigerianPrimarySchool.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName) .IsRequired().HasMaxLength(50);
        builder.Property(u => u.MiddleName).HasMaxLength(50);
        builder.Property(u => u.LastName)  .IsRequired().HasMaxLength(50);
        builder.Property(u => u.Gender)    .HasMaxLength(10);
        builder.Property(u => u.Address)   .HasMaxLength(200);
        builder.Property(u => u.ProfilePictureUrl).HasMaxLength(500);
        builder.Property(u => u.IsActive)  .HasDefaultValue(true);
    }
}
