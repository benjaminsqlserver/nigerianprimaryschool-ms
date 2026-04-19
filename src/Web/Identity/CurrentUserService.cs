using System.Security.Claims;
using NigerianPrimarySchool.Application.Interfaces;

namespace NigerianPrimarySchool.Web.Identity;

//Implementation of ICurrentUserService that retrieves user information from the current HTTP context's User claims principal.
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? P => _httpContextAccessor.HttpContext?.User;

    public string?              UserId          => P?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string?              Email           => P?.FindFirstValue(ClaimTypes.Email);
    public string?              FullName        => P?.FindFirstValue(ClaimTypes.Name);
    public bool                 IsAuthenticated => P?.Identity?.IsAuthenticated ?? false;
    public IEnumerable<string>  Roles           =>
        P?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value) ?? [];
    public bool IsInRole(string role) => P?.IsInRole(role) ?? false;
}
