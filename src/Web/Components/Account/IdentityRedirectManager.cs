using Microsoft.AspNetCore.Components;

namespace NigerianPrimarySchool.Web.Components.Account;

internal sealed class IdentityRedirectManager(NavigationManager navigationManager)
{
    public const string StatusCookieName = "Identity.StatusMessage";
    private static readonly CookieBuilder StatusCookieBuilder = new()
    {
        SameSite    = SameSiteMode.Strict,
        HttpOnly    = true,
        IsEssential = true,
        MaxAge      = TimeSpan.FromSeconds(5),
    };

    public void RedirectTo(string? uri)
    {
        uri ??= "";
        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
            uri = navigationManager.ToBaseRelativePath(uri);
        navigationManager.NavigateTo(uri);
    }

    public void RedirectToWithStatus(string uri, string message, HttpContext context)
    {
        context.Response.Cookies.Append(
            StatusCookieName, message, StatusCookieBuilder.Build(context));
        RedirectTo(uri);
    }

    public void RedirectToCurrentPage() => RedirectTo(navigationManager.Uri);

    public void RedirectToCurrentPageWithStatus(string message, HttpContext context)
        => RedirectToWithStatus(navigationManager.Uri, message, context);
}
