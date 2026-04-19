using Microsoft.AspNetCore.Identity;
using NigerianPrimarySchool.Infrastructure.Identity;

namespace NigerianPrimarySchool.Web.Components.Account;

internal static class IdentityComponentsEndpointRouteBuilderExtensions
{
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var accountGroup = endpoints.MapGroup("/Account");

        accountGroup.MapPost("/Logout", async (
            SignInManager<ApplicationUser> signInManager,
            [Microsoft.AspNetCore.Mvc.FromForm] string? returnUrl) =>
        {
            await signInManager.SignOutAsync();
            return TypedResults.LocalRedirect($"~/{returnUrl ?? "Account/Login"}");
        }).DisableAntiforgery();

        return accountGroup;
    }
}