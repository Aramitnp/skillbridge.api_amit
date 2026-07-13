using System.ComponentModel.DataAnnotations;

namespace SkillBridge.Web.ViewModels;

public sealed class ApplyJobViewModel
{
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;

    [Required, StringLength(2000, MinimumLength = 10), Display(Name = "Cover letter")]
    public string CoverLetter { get; set; } = string.Empty;
}

public sealed class JobApplicationViewModel
{
    public int Id { get; init; }
    public int JobId { get; init; }
    public string JobTitle { get; init; } = string.Empty;
    public string CompanyName { get; init; } = string.Empty;
    public int ApplicantId { get; init; }
    public string ApplicantName { get; init; } = string.Empty;
    public string ApplicantEmail { get; init; } = string.Empty;
    public string CoverLetter { get; init; } = string.Empty;
    public DateTime AppliedDate { get; init; }
    public string Status { get; init; } = string.Empty;
}

public sealed class ApplicantDashboardViewModel
{
    public int TotalApplications { get; init; }
    public int PendingApplications { get; init; }
    public int ShortlistedApplications { get; init; }
    public int RejectedApplications { get; init; }
    public IReadOnlyList<JobApplicationViewModel> RecentApplications { get; init; } = [];
}

public sealed class CompanyDashboardViewModel
{
    public int TotalJobs { get; init; }
    public int ActiveJobs { get; init; }
    public int InactiveJobs { get; init; }
    public int TotalApplicationsReceived { get; init; }
    public IReadOnlyList<JobApplicationViewModel> RecentApplications { get; init; } = [];
}
