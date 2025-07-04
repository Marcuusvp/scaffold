using Microsoft.EntityFrameworkCore;
using ScaffoldApp.Models;

namespace ScaffoldApp.DbConfigurations;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    // Adicione seus DbSets aqui
    public DbSet<LogEntry> LogEntries { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Level).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.Details).HasMaxLength(2000);
        });

        base.OnModelCreating(modelBuilder);
    }
}