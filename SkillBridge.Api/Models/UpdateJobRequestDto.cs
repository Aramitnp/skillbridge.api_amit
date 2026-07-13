using System.ComponentModel.DataAnnotations;

public sealed class UpdateJobRequestDto : IValidatableObject
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    public decimal MinimumSalary { get; set; }

    public decimal MaximumSalary { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(200)]
    public string Location { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    [StringLength(100)]
    public string JobType { get; set; } = string.Empty;

    public DateTime Deadline { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinimumSalary < 0)
        {
            yield return new ValidationResult(
                "Minimum salary cannot be negative.",
                [nameof(MinimumSalary)]);
        }

        if (MaximumSalary < MinimumSalary)
        {
            yield return new ValidationResult(
                "Maximum salary cannot be lower than minimum salary.",
                [nameof(MaximumSalary)]);
        }

        if (Deadline <= DateTime.UtcNow)
        {
            yield return new ValidationResult(
                "Deadline must be later than the current date.",
                [nameof(Deadline)]);
        }
    }
}
