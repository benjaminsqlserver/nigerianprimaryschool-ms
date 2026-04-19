using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Claims;
using NigerianPrimarySchool.Infrastructure.Identity;

namespace NigerianPrimarySchool.Web.Components.Account;

/// <summary>
/// Revalidates auth state on circuit reconnect.
/// Persists auth state from server to WASM via PersistentComponentState.
/// </summary>
internal sealed class PersistingRevalidatingAuthenticationStateProvider
    : RevalidatingServerAuthenticationStateProvider
{
    private readonly IServiceScopeFactory    _scopeFactory;
    private readonly PersistentComponentState _state;
    private readonly IdentityOptions          _options;
    private readonly PersistingComponentStateSubscription _subscription;
    private Task<AuthenticationState>? _authTask;

    public PersistingRevalidatingAuthenticationStateProvider(
        ILoggerFactory               loggerFactory,
        IServiceScopeFactory         serviceScopeFactory,
        PersistentComponentState     persistentComponentState,
        IOptions<IdentityOptions>    optionsAccessor)
        : base(loggerFactory)
    {
        _scopeFactory = serviceScopeFactory;
        _state        = persistentComponentState;
        _options      = optionsAccessor.Value;
        _subscription = _state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
        AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authState, CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        return await ValidateSecurityStampAsync(userManager, authState.User);
    }

    private async Task<bool> ValidateSecurityStampAsync(
        UserManager<ApplicationUser> um, ClaimsPrincipal principal)
    {
        var user = await um.GetUserAsync(principal);
        if (user is null) return false;
        if (!um.SupportsUserSecurityStamp) return true;
        var principalStamp = principal.FindFirstValue(_options.ClaimsIdentity.SecurityStampClaimType);
        var userStamp      = await um.GetSecurityStampAsync(user);
        return principalStamp == userStamp;
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
        => _authTask = task;

    private async Task OnPersistingAsync()
    {
        if (_authTask is null) throw new UnreachableException("Auth task not set.");
        var principal = (await _authTask).User;

        if (principal.Identity?.IsAuthenticated == true)
        {
            var userId   = principal.FindFirst(_options.ClaimsIdentity.UserIdClaimType)?.Value;
            var email    = principal.FindFirst(_options.ClaimsIdentity.EmailClaimType)?.Value;
            var fullName = principal.FindFirst(ClaimTypes.Name)?.Value;
            var roles    = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            if (userId != null && email != null)
            {
                _state.PersistAsJson(nameof(UserInfo), new UserInfo
                {
                    UserId = userId, Email = email,
                    FullName = fullName ?? email, Roles = roles
                });
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        _subscription.Dispose();
        AuthenticationStateChanged -= OnAuthenticationStateChanged;
        base.Dispose(disposing);
    }
}

public sealed class UserInfo
{
    public required string       UserId   { get; init; }
    public required string       Email    { get; init; }
    public required string       FullName { get; init; }
    public          List<string> Roles    { get; init; } = [];
}
