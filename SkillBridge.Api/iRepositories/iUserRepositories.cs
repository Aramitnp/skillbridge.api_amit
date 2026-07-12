using SkillBridge.Api.Entities;

public class UserRepository : IUserRepository
{
    private readonly SkillBridgeDbContext _context;
    public UserRepository(SkillBridgeDbContext context)
    {
        _context = context;
    }
    public Task<string> CreateUserAsync(CreateUserRequestDto req_user)
    {
        User user1 = new User();
        user1.Name = req_user.Name;
        user1.Email = req_user.Email;
        user1.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req_user.Password);
        user1.Type = req_user.Type;

        _context.Users.Add(user1);
        _context.SaveChanges();

        return Task.FromResult("User created successfully!");
    }
}