using Microsoft.EntityFrameworkCore;

namespace ScaffoldApp.DbConfigurations;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    // Adicione seus DbSets aqui
    // public DbSet<Produto> Produtos { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Configurações de modelo
        base.OnModelCreating(modelBuilder);
    }
}