using Microsoft.EntityFrameworkCore;
namespace IAD2026.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Add your DbSets here later
    // public DbSet<YourEntity> YourEntities { get; set; }
}