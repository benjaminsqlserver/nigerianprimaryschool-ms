using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace NigerianPrimarySchool.Web.Client;

/// <summary>
/// Reads auth state persisted from the server by
/// PersistingRevalidatingAuthenticationStateProvider.
/// </summary>
internal sealed class PersistentAuthenticationStateProvider
    : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> Unauthenticated =
        Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> _authState;

    public PersistentAuthenticationStateProvider(PersistentComponentState state)
    {
        if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var info) || info is null)
        {
            _authState = Unauthenticated;
            return;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, info.UserId),
            new(ClaimTypes.Name,           info.FullName),
            new(ClaimTypes.Email,          info.Email),
        };
        claims.AddRange(info.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

        _authState = Task.FromResult(new AuthenticationState(
            new ClaimsPrincipal(
                new ClaimsIdentity(claims, authenticationType: "ServerAuth"))));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => _authState;
}

/// <summary>Mirrors the server-side UserInfo record.</summary>
public sealed class UserInfo
{
    public required string       UserId   { get; init; }
    public required string       Email    { get; init; }
    public required string       FullName { get; init; }
    public          List<string> Roles    { get; init; } = [];
}
