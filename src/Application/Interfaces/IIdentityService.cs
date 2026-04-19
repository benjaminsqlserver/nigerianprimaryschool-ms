using NigerianPrimarySchool.Application.Common.Models;
using NigerianPrimarySchool.Application.DTOs.Auth;

namespace NigerianPrimarySchool.Application.Interfaces;

/// <summary>
/// Contract for Identity operations. Implemented in Infrastructure.
/// Keeps Application layer free of ASP.NET Identity coupling.
/// </summary>
public interface IIdentityService
{
    // ── Create ─────────────────────────────────────────────────────────────
    Task<Result<string>> CreateUserAsync(RegisterUserDto dto);

    // ── Read ───────────────────────────────────────────────────────────────
    Task<UserProfileDto?> GetUserByIdAsync(string userId);
    Task<UserProfileDto?> GetUserByEmailAsync(string email);
    Task<IReadOnlyList<UserListItemDto>> GetAllUsersAsync();
    Task<IReadOnlyList<UserListItemDto>> GetUsersByRoleAsync(string role);

    // ── Roles ──────────────────────────────────────────────────────────────
    Task<Result> AssignRoleAsync(string userId, string role);
    Task<Result> RemoveRoleAsync(string userId, string role);
    Task<IList<string>> GetUserRolesAsync(string userId);
    Task<bool> IsInRoleAsync(string userId, string role);

    // ── Account management ─────────────────────────────────────────────────
    Task<Result> UpdateProfileAsync(string userId, UpdateProfileDto dto);
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordDto dto);
    Task<Result> DeactivateUserAsync(string userId);
    Task<Result> ReactivateUserAsync(string userId);
    Task<Result> DeleteUserAsync(string userId);

    // ── Password reset ─────────────────────────────────────────────────────
    Task<Result<string>> GeneratePasswordResetTokenAsync(string email);
    Task<Result> ResetPasswordAsync(string email, string token, string newPassword);

    // ── Helpers ────────────────────────────────────────────────────────────
    Task<bool> UserExistsAsync(string email);
    Task UpdateLastLoginAsync(string userId);
}
