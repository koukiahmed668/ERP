using ERP.models.inventory;
using Microsoft.EntityFrameworkCore;

namespace ERP.infrastructure.data
{
    public class AppDbContext : DbContext
    {
        public DbSet<InventoryItem> InventoryItems { get; set; }
        // Add other DbSets for different entities (e.g., Sales, HR, etc.)

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships, indexes, or other EF Core-specific configurations here
            modelBuilder.Entity<InventoryItem>()
                .HasIndex(i => i.Name) // Example of creating an index on the Name column
                .IsUnique();
        }
    }
}
