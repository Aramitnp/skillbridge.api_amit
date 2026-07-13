namespace SkillBridge.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<string> CreateUserAsync(CreateUserRequestDto request);
}
