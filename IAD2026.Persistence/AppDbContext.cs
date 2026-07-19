using IAD2026.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace IAD2026.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    // Add your DbSets here later
    public DbSet<OutboxTask> TaskQueue { get; set; }
    public DbSet<DcimData> DcimData => Set<DcimData>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatically apply all configurations in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}