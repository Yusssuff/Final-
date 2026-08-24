using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using SalesBuzz.Shared.Data;

namespace Final_Task.Data
{
    public class AppDbContext : SalesBuzzDbContextBase
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ICurrentBUContext currentBUContext,
            IConfiguration configuration)
            : base(options, currentBUContext)
        {
            var connectionString =
                configuration.GetConnectionString(
                    "DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "ConnectionStrings:DefaultConnection is missing.");
            }

            Database.SetConnectionString(
                connectionString);
        }

        public DbSet<Product> Products =>
            Set<Product>();

        public DbSet<Order> Orders =>
            Set<Order>();

        public DbSet<User> Users =>
            Set<User>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(u => u.Username)
                    .IsUnique();

                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                entity.Property(u => u.Role)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }
}