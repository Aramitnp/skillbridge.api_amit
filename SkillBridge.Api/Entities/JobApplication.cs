using System.ComponentModel.DataAnnotations;

public class JobApplication
{
    [Key]
    public int Id { get; set; }

    public int JobId { get; set; }

    public Job Job { get; set; } = null!;

    public int ApplicantId { get; set; }

    public User Applicant { get; set; } = null!;

    [Required]
    [MaxLength(2000)]
    public string CoverLetter { get; set; } = string.Empty;

    public DateTime AppliedDate { get; set; }

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = string.Empty;
}
