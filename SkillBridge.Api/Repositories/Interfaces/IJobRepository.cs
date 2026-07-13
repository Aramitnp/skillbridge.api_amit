namespace SkillBridge.Api.Repositories.Interfaces;

public interface IJobRepository
{
    Task<PaginatedJobResponseDto> GetActiveJobsAsync(
        int pageNumber,
        int pageSize,
        string? search,
        string? location,
        string? jobType,
        CancellationToken cancellationToken = default);

    Task<JobResponseDto?> GetJobByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JobResponseDto>> GetCompanyJobsAsync(
        int companyId,
        CancellationToken cancellationToken = default);

    Task<int?> GetJobCompanyIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<JobResponseDto> CreateJobAsync(
        int companyId,
        CreateJobRequestDto request,
        CancellationToken cancellationToken = default);

    Task<JobResponseDto?> UpdateJobAsync(
        int id,
        int companyId,
        UpdateJobRequestDto request,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateJobAsync(
        int id,
        int companyId,
        CancellationToken cancellationToken = default);
}
