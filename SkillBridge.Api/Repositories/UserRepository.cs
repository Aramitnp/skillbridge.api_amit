using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Api.Repositories.Interfaces;

namespace SkillBridge.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SkillBridgeDbContext _context;

    public UserRepository(SkillBridgeDbContext context)
    {
        _context = context;
    }

    public async Task<User> CreateUserAsync(
        CreateUserRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (await _context.Users.AnyAsync(
                user => user.Email == normalizedEmail,
                cancellationToken))
        {
            throw new DuplicateEmailException();
        }

        if (!SupportedUserTypes.TryNormalize(request.Type, out var normalizedType))
        {
            throw new ArgumentException("Unsupported user type.", nameof(request));
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Type = normalizedType,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _context.Users.AddAsync(user, cancellationToken);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new DuplicateEmailException(exception);
        }

        return user;
    }
}
