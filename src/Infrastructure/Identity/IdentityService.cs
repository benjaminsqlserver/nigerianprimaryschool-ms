using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NigerianPrimarySchool.Application.Common.Models;
using NigerianPrimarySchool.Application.DTOs.Auth;
using NigerianPrimarySchool.Application.Interfaces;

namespace NigerianPrimarySchool.Infrastructure.Identity;

/// <summary>Implements IIdentityService using ASP.NET Core Identity.</summary>
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser>  _userManager;
    private readonly RoleManager<ApplicationRole>  _roleManager;

    public IdentityService(
        UserManager<ApplicationUser>  userManager,
        RoleManager<ApplicationRole>  roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result<string>> CreateUserAsync(RegisterUserDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
            return Result<string>.Failure($"A user with email '{dto.Email}' already exists.");

        var user = new ApplicationUser
        {
            FirstName   = dto.FirstName.Trim(),
            MiddleName  = dto.MiddleName?.Trim(),
            LastName    = dto.LastName.Trim(),
            Email       = dto.Email.Trim().ToLower(),
            UserName    = dto.Email.Trim().ToLower(),
            PhoneNumber = dto.PhoneNumber,
            DateOfBirth = dto.DateOfBirth,
            Gender      = dto.Gender,
            Address     = dto.Address,
            IsActive    = true,
            CreatedAt   = DateTime.UtcNow,
        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
            return Result<string>.Failure(createResult.Errors.Select(e => e.Description));

        if (!string.IsNullOrWhiteSpace(dto.Role))
        {
            var roleResult = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!roleResult.Succeeded)
                return Result<string>.Failure(roleResult.Errors.Select(e => e.Description));
        }

        return Result<string>.Success(user.Id);
    }

    public async Task<UserProfileDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is null ? null : await MapToProfileAsync(user);
    }

    public async Task<UserProfileDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is null ? null : await MapToProfileAsync(user);
    }

    public async Task<IReadOnlyList<UserListItemDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
            .ToListAsync();
        return await MapToListItemsAsync(users);
    }

    public async Task<IReadOnlyList<UserListItemDto>> GetUsersByRoleAsync(string role)
    {
        var users = await _userManager.GetUsersInRoleAsync(role);
        return await MapToListItemsAsync(users.OrderBy(u => u.LastName).ToList());
    }

    public async Task<Result> AssignRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Result.Failure("User not found.");
        if (!await _roleManager.RoleExistsAsync(role))
            return Result.Failure($"Role '{role}' does not exist.");
        var result = await _userManager.AddToRoleAsync(user, role);
        return result.Succeeded ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<Result> RemoveRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Result.Failure("User not found.");
        var result = await _userManager.RemoveFromRoleAsync(user, role);
        return result.Succeeded ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is null ? [] : await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is not null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Result.Failure("User not found.");
        user.FirstName      = dto.FirstName.Trim();
        user.MiddleName     = dto.MiddleName?.Trim();
        user.LastName       = dto.LastName.Trim();
        user.PhoneNumber    = dto.PhoneNumber;
        user.Gender         = dto.Gender;
        user.DateOfBirth    = dto.DateOfBirth;
        user.Address        = dto.Address;
        user.LastModifiedAt = DateTime.UtcNow;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Result.Failure("User not found.");
        var result = await _userManager.ChangePasswordAsync(
            user, dto.CurrentPassword, dto.NewPassword);
        return result.Succeeded ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<Result> DeactivateUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Result.Failure("User not found.");
        user.IsActive = false; user.LastModifiedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        return Result.Success();
    }

    public async Task<Result> ReactivateUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Result.Failure("User not found.");
        user.IsActive = true; user.LastModifiedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        return Result.Success();
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Result.Failure("User not found.");
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<Result<string>> GeneratePasswordResetTokenAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return Result<string>.Failure("No account with that email.");
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return Result<string>.Success(token);
    }

    public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return Result.Failure("No account with that email.");
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<bool> UserExistsAsync(string email) =>
        await _userManager.FindByEmailAsync(email) is not null;

    public async Task UpdateLastLoginAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return;
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
    }

    // ── Mapping helpers ────────────────────────────────────────────────────
    private async Task<UserProfileDto> MapToProfileAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return new UserProfileDto
        {
            Id = user.Id, FullName = user.FullName,
            FirstName = user.FirstName, MiddleName = user.MiddleName, LastName = user.LastName,
            Email = user.Email ?? string.Empty, PhoneNumber = user.PhoneNumber,
            Gender = user.Gender, DateOfBirth = user.DateOfBirth, Address = user.Address,
            ProfilePictureUrl = user.ProfilePictureUrl,
            IsActive = user.IsActive, CreatedAt = user.CreatedAt, LastLoginAt = user.LastLoginAt,
            Roles = [.. roles],
        };
    }

    private async Task<IReadOnlyList<UserListItemDto>> MapToListItemsAsync(IList<ApplicationUser> users)
    {
        var result = new List<UserListItemDto>(users.Count);
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserListItemDto
            {
                Id = user.Id, FullName = user.FullName,
                Email = user.Email ?? string.Empty, PhoneNumber = user.PhoneNumber,
                PrimaryRole = roles.FirstOrDefault() ?? "—",
                IsActive = user.IsActive, CreatedAt = user.CreatedAt, LastLoginAt = user.LastLoginAt,
            });
        }
        return result;
    }
}
