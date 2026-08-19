using Microsoft.EntityFrameworkCore;
using SalesBuzz.Shared.Data;

namespace Final_Task.Data;

public class AppDbContext : SalesBuzzDbContextBase
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ICurrentBUContext currentBUContext)
        : base(options, currentBUContext)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
} 