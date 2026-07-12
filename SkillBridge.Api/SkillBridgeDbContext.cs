using Microsoft.EntityFrameworkCore;

public class SkillBridgeDbContext : DbContext
{
    public SkillBridgeDbContext(
        DbContextOptions<SkillBridgeDbContext> options
    ) : base(options)
    {
    }

    // Define your DbSets here, for example:
    // public DbSet<User> Users { get; set; }

    public DbSet<User> Users { get; set; }
}