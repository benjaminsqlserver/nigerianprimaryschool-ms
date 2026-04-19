using System.ComponentModel.DataAnnotations;

namespace NigerianPrimarySchool.Application.DTOs.Auth;

// ── User profile shown after login ────────────────────────────────────────────
public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public IList<string> Roles { get; set; } = [];
    public string PrimaryRole => Roles.FirstOrDefault() ?? "—";
}

// ── Change password form ───────────────────────────────────────────────────────
public class ChangePasswordDto
{
    [Required]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8,
        ErrorMessage = "Password must be at least 8 characters")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
    [DataType(DataType.Password)]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

// ── Update profile form ────────────────────────────────────────────────────────
public class UpdateProfileDto
{
    [Required][StringLength(50)] public string FirstName { get; set; } = string.Empty;
    [StringLength(50)]           public string? MiddleName { get; set; }
    [Required][StringLength(50)] public string LastName { get; set; } = string.Empty;
    [Phone]                      public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
}

// ── User list item (for admin table) ─────────────────────────────────────────
public class UserListItemDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string PrimaryRole { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
