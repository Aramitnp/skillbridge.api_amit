namespace SkillBridge.Api.Repositories.Interfaces;

public interface IJobRepository
{
    Task<IEnumerable<JobDto>> GetJobListAsync();
    Task<JobDto> GetJobByIdAsync(int id);
}
