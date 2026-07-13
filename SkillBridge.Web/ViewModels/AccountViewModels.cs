using System.ComponentModel.DataAnnotations;

namespace SkillBridge.Web.ViewModels;

public sealed class RegisterViewModel
{
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(320)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8), DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = "Applicant";
}

public sealed class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

public sealed class LoginApiResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public int UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
}

public sealed class RegistrationApiResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
