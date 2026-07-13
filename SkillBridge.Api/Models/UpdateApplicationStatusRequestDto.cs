using System.ComponentModel.DataAnnotations;

public sealed class UpdateApplicationStatusRequestDto : IValidatableObject
{
    [Required(AllowEmptyStrings = false)]
    public string Status { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(Status) &&
            !ApplicationStatuses.TryNormalize(Status, out _))
        {
            yield return new ValidationResult(
                "Status must be Pending, Reviewed, Shortlisted, Rejected, or Accepted.",
                [nameof(Status)]);
        }
    }
}
