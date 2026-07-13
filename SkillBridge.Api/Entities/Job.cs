using System.ComponentModel.DataAnnotations;

public class Job
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    public decimal MinimumSalary { get; set; }

    public decimal MaximumSalary { get; set; }

    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string JobType { get; set; } = string.Empty;

    public DateTime PostedDate { get; set; }

    public DateTime Deadline { get; set; }

    public bool IsActive { get; set; }

    public int CompanyId { get; set; }

    public User Company { get; set; } = null!;

    public ICollection<JobApplication> Applications { get; set; } =
        new List<JobApplication>();
}
