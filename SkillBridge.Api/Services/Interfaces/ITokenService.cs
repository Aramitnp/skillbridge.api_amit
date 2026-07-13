using SkillBridge.Api.Repositories;

namespace SkillBridge.Api.Services.Interfaces;

public interface ITokenService
{
    AccessTokenResult CreateAccessToken(UserLoginResult user);
}

public sealed record AccessTokenResult(string Value, DateTime ExpiresAt);
