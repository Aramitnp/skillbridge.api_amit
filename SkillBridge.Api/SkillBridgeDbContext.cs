using Microsoft.EntityFrameworkCore;

public class SkillBridgeDbContext : DbContext
{
    public SkillBridgeDbContext(
        DbContextOptions<SkillBridgeDbContext> options
    ) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();
    }
}
