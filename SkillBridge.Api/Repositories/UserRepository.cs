using SkillBridge.Api.Repositories.Interfaces;

namespace SkillBridge.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SkillBridgeDbContext _context;

    public UserRepository(SkillBridgeDbContext context)
    {
        _context = context;
    }

    public async Task<string> CreateUserAsync(CreateUserRequestDto request)
    {
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Type = request.Type,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return "User created successfully!";
    }
}
