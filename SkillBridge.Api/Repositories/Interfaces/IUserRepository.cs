namespace SkillBridge.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User> CreateUserAsync(
        CreateUserRequestDto request,
        CancellationToken cancellationToken = default);
}
