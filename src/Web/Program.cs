using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using NigerianPrimarySchool.Application.Interfaces;
using NigerianPrimarySchool.Domain.Constants;
using NigerianPrimarySchool.Infrastructure;
using NigerianPrimarySchool.Infrastructure.Identity;
using NigerianPrimarySchool.Infrastructure.Persistence;
using NigerianPrimarySchool.Web.Components;
using NigerianPrimarySchool.Web.Components.Account;
using NigerianPrimarySchool.Web.Identity;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// ── Infrastructure (EF Core + Identity) ──────────────────────────────────
builder.Services.AddInfrastructure(builder.Configuration);

// ── Current user ──────────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// ── Blazor Auto ───────────────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// ── Blazor Identity plumbing ──────────────────────────────────────────────
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider,
    PersistingRevalidatingAuthenticationStateProvider>();

// ── Cookie authentication ─────────────────────────────────────────────────
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath        = "/Account/Login";
    options.LogoutPath       = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan   = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// ── Authorization Policies ────────────────────────────────────────────────
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.RequireSuperAdmin,
        p => p.RequireRole(Roles.SuperAdmin));

    options.AddPolicy(Policies.RequireHeadTeacherOrAbove,
        p => p.RequireRole(Roles.SuperAdmin, Roles.HeadTeacher));

    options.AddPolicy(Policies.RequireAnyStaff,
        p => p.RequireRole([.. Roles.AllStaff]));

    options.AddPolicy(Policies.RequireTeacher,
        p => p.RequireRole([.. Roles.TeachingRoles]));

    options.AddPolicy(Policies.RequireFinance,
        p => p.RequireRole([.. Roles.FinanceRoles]));

    options.AddPolicy(Policies.RequireStore,
        p => p.RequireRole([.. Roles.StoreRoles]));

    options.AddPolicy(Policies.RequireParentOrStudent,
        p => p.RequireRole(Roles.Parent, Roles.Student));

    options.AddPolicy(Policies.RequireParent,
        p => p.RequireRole(Roles.Parent));

    options.AddPolicy(Policies.RequireStudent,
        p => p.RequireRole(Roles.Student));
});

// ── Radzen ────────────────────────────────────────────────────────────────
builder.Services.AddRadzenComponents();

var app = builder.Build();

// ── Seed database ─────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    try
    {
        await DatabaseSeeder.SeedAsync(
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(),
            scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(),
            scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>(),
            scope.ServiceProvider.GetRequiredService<ILogger<Program>>());
    }
    catch (Exception ex)
    {
        scope.ServiceProvider.GetRequiredService<ILogger<Program>>()
            .LogError(ex, "Database seeding failed.");
    }
}

// ── Pipeline ──────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
    app.UseWebAssemblyDebugging();
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(NigerianPrimarySchool.Web.Client._Imports).Assembly);

app.MapAdditionalIdentityEndpoints();

app.Run();
