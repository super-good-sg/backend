using Microsoft.EntityFrameworkCore;

namespace SuperGood.Jobs.Persistence;

/// <summary>Everything lives in the <c>jobs</c> schema (ADR 0005). The runtime
/// database user has row-level rights there and nowhere else.</summary>
public sealed class JobsDbContext(DbContextOptions<JobsDbContext> options) : DbContext(options)
{
    public const string SchemaName = "jobs";

    public DbSet<Job> Jobs => Set<Job>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        modelBuilder.Entity<Job>(e =>
        {
            e.ToTable("jobs", t =>
            {
                t.HasCheckConstraint("ck_jobs_type", "\"Type\" IN ('OrderExtraction', 'ProductExtraction')");
                t.HasCheckConstraint("ck_jobs_status", "\"Status\" IN ('Pending', 'Processing', 'Succeeded', 'Failed')");
                t.HasCheckConstraint("ck_jobs_result", "(\"Status\" = 'Succeeded') = (\"Result\" IS NOT NULL)");
                t.HasCheckConstraint("ck_jobs_error", "(\"Status\" = 'Failed') = (\"Error\" IS NOT NULL)");
            });
            e.HasKey(j => j.Id);
            e.Property(j => j.Type).HasConversion<string>().HasMaxLength(32);
            e.Property(j => j.Status).HasConversion<string>().HasMaxLength(16);
            e.Property(j => j.Result).HasColumnType("jsonb");
            e.Property(j => j.Error).HasColumnType("jsonb");
            e.HasIndex(j => new { j.OwnerId, j.CreatedAt });
            e.HasIndex(j => j.Status);
        });
    }
}
