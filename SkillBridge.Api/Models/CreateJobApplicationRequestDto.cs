using System.ComponentModel.DataAnnotations;

public sealed class CreateJobApplicationRequestDto
{
    [Range(1, int.MaxValue)]
    public int JobId { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(2000, MinimumLength = 10)]
    public string CoverLetter { get; set; } = string.Empty;
}
