using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Api.Repositories.Interfaces;

namespace SkillBridge.Api.Repositories;

public class JobApplicationRepository : IJobApplicationRepository
{
    private readonly SkillBridgeDbContext _context;

    public JobApplicationRepository(SkillBridgeDbContext context)
    {
        _context = context;
    }

    public async Task<JobApplicationResponseDto> ApplyAsync(
        int applicantId,
        CreateJobApplicationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs
            .AsNoTracking()
            .SingleOrDefaultAsync(
                existingJob => existingJob.Id == request.JobId,
                cancellationToken);

        if (job is null)
        {
            throw new JobNotFoundException();
        }

        if (!job.IsActive || job.Deadline <= DateTime.UtcNow)
        {
            throw new JobUnavailableException();
        }

        if (await _context.JobApplications.AnyAsync(
                application => application.JobId == request.JobId &&
                    application.ApplicantId == applicantId,
                cancellationToken))
        {
            throw new DuplicateJobApplicationException();
        }

        var application = new JobApplication
        {
            JobId = request.JobId,
            ApplicantId = applicantId,
            CoverLetter = request.CoverLetter.Trim(),
            AppliedDate = DateTime.UtcNow,
            Status = ApplicationStatuses.Pending
        };

        await _context.JobApplications.AddAsync(application, cancellationToken);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new DuplicateJobApplicationException(exception);
        }

        return await ToResponse(_context.JobApplications.AsNoTracking())
            .SingleAsync(
                savedApplication => savedApplication.Id == application.Id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<JobApplicationResponseDto>> GetApplicantApplicationsAsync(
        int applicantId,
        CancellationToken cancellationToken = default)
    {
        return await ToResponse(_context.JobApplications.AsNoTracking())
            .Where(application => application.ApplicantId == applicantId)
            .OrderByDescending(application => application.AppliedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<JobApplicationResponseDto>> GetReceivedApplicationsAsync(
        int companyId,
        int? jobId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.JobApplications
            .AsNoTracking()
            .Where(application => application.Job.CompanyId == companyId);

        if (jobId.HasValue)
        {
            query = query.Where(application => application.JobId == jobId.Value);
        }

        return await ToResponse(query)
            .OrderByDescending(application => application.AppliedDate)
            .ToListAsync(cancellationToken);
    }

    public Task<int?> GetApplicationCompanyIdAsync(
        int applicationId,
        CancellationToken cancellationToken = default)
    {
        return _context.JobApplications
            .AsNoTracking()
            .Where(application => application.Id == applicationId)
            .Select(application => (int?)application.Job.CompanyId)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<JobApplicationResponseDto?> UpdateStatusAsync(
        int applicationId,
        int companyId,
        string status,
        CancellationToken cancellationToken = default)
    {
        var application = await _context.JobApplications
            .Include(existingApplication => existingApplication.Job)
            .SingleOrDefaultAsync(
                existingApplication => existingApplication.Id == applicationId &&
                    existingApplication.Job.CompanyId == companyId,
                cancellationToken);

        if (application is null)
        {
            return null;
        }

        if (!ApplicationStatuses.TryNormalize(status, out var normalizedStatus))
        {
            throw new ArgumentException("Unsupported application status.", nameof(status));
        }

        application.Status = normalizedStatus;
        await _context.SaveChangesAsync(cancellationToken);

        return await ToResponse(_context.JobApplications.AsNoTracking())
            .SingleAsync(
                updatedApplication => updatedApplication.Id == applicationId,
                cancellationToken);
    }

    public async Task<ApplicantDashboardDto> GetApplicantDashboardAsync(
        int applicantId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.JobApplications
            .AsNoTracking()
            .Where(application => application.ApplicantId == applicantId);

        return new ApplicantDashboardDto
        {
            TotalApplications = await query.CountAsync(cancellationToken),
            PendingApplications = await query.CountAsync(
                application => application.Status == ApplicationStatuses.Pending,
                cancellationToken),
            ShortlistedApplications = await query.CountAsync(
                application => application.Status == ApplicationStatuses.Shortlisted,
                cancellationToken),
            RejectedApplications = await query.CountAsync(
                application => application.Status == ApplicationStatuses.Rejected,
                cancellationToken),
            RecentApplications = await ToResponse(query)
                .OrderByDescending(application => application.AppliedDate)
                .Take(5)
                .ToListAsync(cancellationToken)
        };
    }

    public async Task<CompanyDashboardDto> GetCompanyDashboardAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        var jobs = _context.Jobs
            .AsNoTracking()
            .Where(job => job.CompanyId == companyId);
        var applications = _context.JobApplications
            .AsNoTracking()
            .Where(application => application.Job.CompanyId == companyId);

        return new CompanyDashboardDto
        {
            TotalJobs = await jobs.CountAsync(cancellationToken),
            ActiveJobs = await jobs.CountAsync(
                job => job.IsActive,
                cancellationToken),
            InactiveJobs = await jobs.CountAsync(
                job => !job.IsActive,
                cancellationToken),
            TotalApplicationsReceived = await applications.CountAsync(cancellationToken),
            RecentApplications = await ToResponse(applications)
                .OrderByDescending(application => application.AppliedDate)
                .Take(5)
                .ToListAsync(cancellationToken)
        };
    }

    private static IQueryable<JobApplicationResponseDto> ToResponse(
        IQueryable<JobApplication> applications)
    {
        return applications.Select(application => new JobApplicationResponseDto
        {
            Id = application.Id,
            JobId = application.JobId,
            JobTitle = application.Job.Title,
            ApplicantId = application.ApplicantId,
            ApplicantName = application.Applicant.Name,
            ApplicantEmail = application.Applicant.Email,
            CoverLetter = application.CoverLetter,
            AppliedDate = application.AppliedDate,
            Status = application.Status
        });
    }
}
