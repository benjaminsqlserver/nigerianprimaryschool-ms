using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NigerianPrimarySchool.Domain.Constants;
using NigerianPrimarySchool.Infrastructure.Identity;

namespace NigerianPrimarySchool.Infrastructure.Persistence;

/// <summary>Seeds roles and the default SuperAdmin. Call from Program.cs before app.Run().</summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext           context,
        UserManager<ApplicationUser>   userManager,
        RoleManager<ApplicationRole>   roleManager,
        ILogger                        logger)
    {
        // 1. Apply pending migrations
        if ((await context.Database.GetPendingMigrationsAsync()).Any())
        {
            logger.LogInformation("Applying pending migrations…");
            await context.Database.MigrateAsync();
        }

        // 2. Seed roles
        var descriptions = new Dictionary<string, string>
        {
            [Roles.SuperAdmin]           = "Full unrestricted access to the entire application",
            [Roles.HeadTeacher]          = "School principal — academic & administrative oversight",
            [Roles.SubjectTeacher]       = "Teacher assigned to specific subjects across classes",
            [Roles.ClassTeacher]         = "Teacher responsible for a particular class/arm",
            [Roles.SchoolAccountant]     = "Manages school fees, levies and financial records",
            [Roles.SchoolBursar]         = "Bursary operations — same permissions as Accountant",
            [Roles.SchoolBookShopKeeper] = "Manages the school bookshop — stock and sales",
            [Roles.SchoolStoreKeeper]    = "Manages general school store — supplies and equipment",
            [Roles.Parent]               = "Parent or legal guardian of enrolled students",
            [Roles.Student]              = "Enrolled pupil — read-only access to own records",
        };

        foreach (var (name, desc) in descriptions)
        {
            if (!await roleManager.RoleExistsAsync(name))
            {
                var result = await roleManager.CreateAsync(new ApplicationRole(name, desc));
                if (result.Succeeded)
                    logger.LogInformation("Created role: {Role}", name);
                else
                    logger.LogError("Failed to create role {Role}: {Errors}", name,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // 3. Seed default SuperAdmin
        const string email    = "superadmin@school.edu.ng";
        const string password = "Admin@1234!";  // CHANGE IN PRODUCTION

        if (await userManager.FindByEmailAsync(email) is null)
        {
            var admin = new ApplicationUser
            {
                FirstName = "System", LastName = "Administrator",
                UserName = email, Email = email,
                PhoneNumber = "+2348000000000",
                IsActive = true, EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
            };
            var cr = await userManager.CreateAsync(admin, password);
            if (cr.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.SuperAdmin);
                logger.LogWarning(
                    "DEFAULT SUPERADMIN CREATED — Email: {Email} Password: {Pwd} — CHANGE IMMEDIATELY!",
                    email, password);
            }
        }
    }
}
