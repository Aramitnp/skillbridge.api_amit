using Microsoft.EntityFrameworkCore;

public class SkillBridgeDbContext : DbContext
{
    public SkillBridgeDbContext(
        DbContextOptions<SkillBridgeDbContext> options
    ) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<Job>(entity =>
        {
            entity.Property(job => job.MinimumSalary).HasPrecision(18, 2);
            entity.Property(job => job.MaximumSalary).HasPrecision(18, 2);

            entity.HasOne(job => job.Company)
                .WithMany(user => user.Jobs)
                .HasForeignKey(job => job.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(job => job.CompanyId);
            entity.HasIndex(job => job.IsActive);
            entity.HasIndex(job => job.JobType);
        });

        modelBuilder.Entity<JobApplication>(entity =>
        {
            entity.HasOne(application => application.Job)
                .WithMany(job => job.Applications)
                .HasForeignKey(application => application.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(application => application.Applicant)
                .WithMany(user => user.JobApplications)
                .HasForeignKey(application => application.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(application => application.ApplicantId);
            entity.HasIndex(application => new
                {
                    application.JobId,
                    application.ApplicantId
                })
                .IsUnique();
        });
    }
}
