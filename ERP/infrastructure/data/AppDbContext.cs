using ERP.models.inventory;
using ERP.models.user;
using Microsoft.EntityFrameworkCore;

namespace ERP.infrastructure.data
{
    public class AppDbContext : DbContext
    {
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<User> Users { get; set; } // Add DbSet for User entity

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure InventoryItem entity
            modelBuilder.Entity<InventoryItem>()
                .HasIndex(i => i.Name)
                .IsUnique();

            // Configure User entity
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique(); // Enforce unique usernames
        }
    }
}
