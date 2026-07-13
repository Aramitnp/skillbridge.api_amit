namespace SkillBridge.Api.Repositories;

public enum UserLoginStatus
{
    Success,
    InvalidCredentials,
    Inactive
}

public sealed class UserLoginResult
{
    public UserLoginStatus Status { get; private init; }
    public int UserId { get; private init; }
    public string Name { get; private init; } = string.Empty;
    public string Email { get; private init; } = string.Empty;
    public string Type { get; private init; } = string.Empty;

    public static UserLoginResult Success(
        int userId,
        string name,
        string email,
        string type) => new()
        {
            Status = UserLoginStatus.Success,
            UserId = userId,
            Name = name,
            Email = email,
            Type = type
        };

    public static UserLoginResult InvalidCredentials() => new()
    {
        Status = UserLoginStatus.InvalidCredentials
    };

    public static UserLoginResult Inactive() => new()
    {
        Status = UserLoginStatus.Inactive
    };
}
