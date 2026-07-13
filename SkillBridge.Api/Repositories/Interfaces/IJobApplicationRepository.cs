namespace SkillBridge.Api.Repositories.Interfaces;

public interface IJobApplicationRepository
{
    Task<JobApplicationResponseDto> ApplyAsync(
        int applicantId,
        CreateJobApplicationRequestDto request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JobApplicationResponseDto>> GetApplicantApplicationsAsync(
        int applicantId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JobApplicationResponseDto>> GetReceivedApplicationsAsync(
        int companyId,
        int? jobId = null,
        CancellationToken cancellationToken = default);

    Task<int?> GetApplicationCompanyIdAsync(
        int applicationId,
        CancellationToken cancellationToken = default);

    Task<JobApplicationResponseDto?> UpdateStatusAsync(
        int applicationId,
        int companyId,
        string status,
        CancellationToken cancellationToken = default);

    Task<ApplicantDashboardDto> GetApplicantDashboardAsync(
        int applicantId,
        CancellationToken cancellationToken = default);

    Task<CompanyDashboardDto> GetCompanyDashboardAsync(
        int companyId,
        CancellationToken cancellationToken = default);
}
