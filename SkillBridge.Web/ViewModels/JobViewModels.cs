using System.ComponentModel.DataAnnotations;

namespace SkillBridge.Web.ViewModels;

public sealed class JobViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal MinimumSalary { get; init; }
    public decimal MaximumSalary { get; init; }
    public string Location { get; init; } = string.Empty;
    public string JobType { get; init; } = string.Empty;
    public DateTime PostedDate { get; init; }
    public DateTime Deadline { get; init; }
    public bool IsActive { get; init; }
    public int CompanyId { get; init; }
    public string CompanyName { get; init; } = string.Empty;
}

public sealed class PaginatedJobResponse
{
    public int CurrentPage { get; init; }
    public int PageSize { get; init; }
    public int TotalRecords { get; init; }
    public int TotalPages { get; init; }
    public IReadOnlyList<JobViewModel> Items { get; init; } = [];
}

public sealed class JobListViewModel
{
    public IReadOnlyList<JobViewModel> Jobs { get; init; } = [];
    public string? Search { get; init; }
    public string? Location { get; init; }
    public string? JobType { get; init; }
    public int CurrentPage { get; init; } = 1;
    public int TotalPages { get; init; }
    public int TotalRecords { get; init; }
}

public sealed class JobDetailsViewModel
{
    public required JobViewModel Job { get; init; }
    public bool IsLoggedIn { get; init; }
    public string? UserType { get; init; }
}

public class JobFormViewModel : IValidatableObject
{
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Minimum salary"), Range(0, double.MaxValue)]
    public decimal MinimumSalary { get; set; }

    [Display(Name = "Maximum salary"), Range(0, double.MaxValue)]
    public decimal MaximumSalary { get; set; }

    [Required, StringLength(200)]
    public string Location { get; set; } = string.Empty;

    [Required, StringLength(100), Display(Name = "Job type")]
    public string JobType { get; set; } = string.Empty;

    [Required, DataType(DataType.DateTime)]
    public DateTime Deadline { get; set; } = DateTime.Now.AddDays(30);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MaximumSalary < MinimumSalary)
        {
            yield return new ValidationResult(
                "Maximum salary cannot be lower than minimum salary.",
                [nameof(MaximumSalary)]);
        }

        if (Deadline.ToUniversalTime() <= DateTime.UtcNow)
        {
            yield return new ValidationResult("Deadline must be in the future.", [nameof(Deadline)]);
        }
    }
}

public sealed class EditJobViewModel : JobFormViewModel
{
    public int Id { get; set; }
}

public sealed class HomeViewModel
{
    public IReadOnlyList<JobViewModel> RecentJobs { get; init; } = [];
    public string? Notice { get; init; }
}
