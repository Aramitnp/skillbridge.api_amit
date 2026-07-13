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
    public async Task<IEnumerable<JobDto>> GetJobListAsync()
    {
        var jobList = await _context.Jobs.ToListAsync();
        if (jobList != null)
        {
            return jobList.Select(job => new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Company = job.Company,
                Location = job.Location,
                JobType = job.JobType,
                MaximumSalary = job.MaximumSalary,
                MinimumSalary = job.MinimumSalary,
                PostedDate = job.PostedDate,
                DeadLineDate = job.DeadLineDate,
                isActive = job.isActive
            }).ToList();
        }
        return new List<JobDto>();
    }
    public async Task<JobDto> GetJobByIdAsync(int id)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);
        // select * from Jobs where Id = id
        if (job != null)
        {
            return new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Company = job.Company,
                Location = job.Location,
                JobType = job.JobType,
                MaximumSalary = job.MaximumSalary,
                MinimumSalary = job.MinimumSalary,
                PostedDate = job.PostedDate,
                DeadLineDate = job.DeadLineDate,
                isActive = job.isActive
            };
        }
        return new JobDto();
    }
}
