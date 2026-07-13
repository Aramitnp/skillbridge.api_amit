public sealed class LoginResponseDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public int UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
}
