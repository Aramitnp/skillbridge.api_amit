using System.ComponentModel.DataAnnotations;

public sealed class LoginRequestDto
{
    private string _email = string.Empty;

    [Required(AllowEmptyStrings = false)]
    [EmailAddress]
    [StringLength(450)]
    public string Email
    {
        get => _email;
        set => _email = value?.Trim().ToLowerInvariant() ?? string.Empty;
    }

    [Required(AllowEmptyStrings = false)]
    public string Password { get; set; } = string.Empty;
}
