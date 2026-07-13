using System.ComponentModel.DataAnnotations;

public class CreateUserRequestDto : IValidatableObject
{
    private string _name = string.Empty;
    private string _email = string.Empty;
    private string _type = string.Empty;

    [Required(AllowEmptyStrings = false)]
    [StringLength(100, MinimumLength = 2)]
    public string Name
    {
        get => _name;
        set => _name = value?.Trim() ?? string.Empty;
    }

    [Required(AllowEmptyStrings = false)]
    [EmailAddress]
    [StringLength(450)]
    public string Email
    {
        get => _email;
        set => _email = value?.Trim().ToLowerInvariant() ?? string.Empty;
    }

    [Required(AllowEmptyStrings = false)]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    public string Type
    {
        get => _type;
        set => _type = value?.Trim() ?? string.Empty;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(Type) &&
            !SupportedUserTypes.TryNormalize(Type, out _))
        {
            yield return new ValidationResult(
                $"Type must be either {SupportedUserTypes.Applicant} or {SupportedUserTypes.Company}.",
                [nameof(Type)]);
        }
    }
}
