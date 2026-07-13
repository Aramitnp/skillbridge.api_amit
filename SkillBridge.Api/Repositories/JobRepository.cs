using Microsoft.EntityFrameworkCore;
using SkillBridge.Api.Repositories.Interfaces;

namespace SkillBridge.Api.Repositories;

public class JobRepository : IJobRepository
{
    private readonly SkillBridgeDbContext _context;

    public JobRepository(SkillBridgeDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedJobResponseDto> GetActiveJobsAsync(
        int pageNumber,
        int pageSize,
        string? search,
        string? location,
        string? jobType,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Jobs
            .AsNoTracking()
            .Where(job => job.IsActive && job.Deadline > DateTime.UtcNow);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();
            query = query.Where(job =>
                job.Title.Contains(value) ||
                job.Location.Contains(value) ||
                job.Company.Name.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            var value = location.Trim();
            query = query.Where(job => job.Location.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(jobType))
        {
            var value = jobType.Trim();
            query = query.Where(job => job.JobType == value);
        }

        var totalRecords = await query.CountAsync(cancellationToken);
        var items = await ToResponse(query)
            .OrderByDescending(job => job.PostedDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedJobResponseDto
        {
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
            Items = items
        };
    }

    public Task<JobResponseDto?> GetJobByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return ToResponse(_context.Jobs.AsNoTracking())
            .SingleOrDefaultAsync(
                job => job.Id == id && job.IsActive,
                cancellationToken);
    }

    public async Task<IReadOnlyList<JobResponseDto>> GetCompanyJobsAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        return await ToResponse(_context.Jobs.AsNoTracking())
            .Where(job => job.CompanyId == companyId)
            .OrderByDescending(job => job.PostedDate)
            .ToListAsync(cancellationToken);
    }

    public Task<int?> GetJobCompanyIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.Jobs
            .AsNoTracking()
            .Where(job => job.Id == id)
            .Select(job => (int?)job.CompanyId)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<JobResponseDto> CreateJobAsync(
        int companyId,
        CreateJobRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var job = new Job
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            MinimumSalary = request.MinimumSalary,
            MaximumSalary = request.MaximumSalary,
            Location = request.Location.Trim(),
            JobType = request.JobType.Trim(),
            PostedDate = DateTime.UtcNow,
            Deadline = request.Deadline.ToUniversalTime(),
            IsActive = true,
            CompanyId = companyId
        };

        await _context.Jobs.AddAsync(job, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return await ToResponse(_context.Jobs.AsNoTracking())
            .SingleAsync(createdJob => createdJob.Id == job.Id, cancellationToken);
    }

    public async Task<JobResponseDto?> UpdateJobAsync(
        int id,
        int companyId,
        UpdateJobRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs
            .SingleOrDefaultAsync(
                existingJob => existingJob.Id == id &&
                    existingJob.CompanyId == companyId,
                cancellationToken);

        if (job is null)
        {
            return null;
        }

        job.Title = request.Title.Trim();
        job.Description = request.Description.Trim();
        job.MinimumSalary = request.MinimumSalary;
        job.MaximumSalary = request.MaximumSalary;
        job.Location = request.Location.Trim();
        job.JobType = request.JobType.Trim();
        job.Deadline = request.Deadline.ToUniversalTime();

        await _context.SaveChangesAsync(cancellationToken);

        return await ToResponse(_context.Jobs.AsNoTracking())
            .SingleAsync(updatedJob => updatedJob.Id == id, cancellationToken);
    }

    public async Task<bool> DeactivateJobAsync(
        int id,
        int companyId,
        CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs
            .SingleOrDefaultAsync(
                existingJob => existingJob.Id == id &&
                    existingJob.CompanyId == companyId,
                cancellationToken);

        if (job is null)
        {
            return false;
        }

        job.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static IQueryable<JobResponseDto> ToResponse(IQueryable<Job> jobs)
    {
        return jobs.Select(job => new JobResponseDto
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            MinimumSalary = job.MinimumSalary,
            MaximumSalary = job.MaximumSalary,
            Location = job.Location,
            JobType = job.JobType,
            PostedDate = job.PostedDate,
            Deadline = job.Deadline,
            IsActive = job.IsActive,
            CompanyId = job.CompanyId,
            CompanyName = job.Company.Name
        });
    }
}
