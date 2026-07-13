public sealed class ApplicantDashboardDto
{
    public int TotalApplications { get; init; }
    public int PendingApplications { get; init; }
    public int ShortlistedApplications { get; init; }
    public int RejectedApplications { get; init; }
    public IReadOnlyList<JobApplicationResponseDto> RecentApplications { get; init; } =
        Array.Empty<JobApplicationResponseDto>();
}

public sealed class CompanyDashboardDto
{
    public int TotalJobs { get; init; }
    public int ActiveJobs { get; init; }
    public int InactiveJobs { get; init; }
    public int TotalApplicationsReceived { get; init; }
    public IReadOnlyList<JobApplicationResponseDto> RecentApplications { get; init; } =
        Array.Empty<JobApplicationResponseDto>();
}
