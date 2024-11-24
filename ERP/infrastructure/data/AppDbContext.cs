using ERP.models.inventory;
using ERP.models.sales;
using ERP.models.user;
using Microsoft.EntityFrameworkCore;

namespace ERP.infrastructure.data
{
    public class AppDbContext : DbContext
    {
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }  // Add this line to include SaleItem


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

            // Configure SaleItem entity
            modelBuilder.Entity<SaleItem>()
                .HasKey(si => si.ItemId); // Set ItemId as primary key
            modelBuilder.Entity<SaleItem>()
                .HasOne(si => si.InventoryItem)  // Navigation property to InventoryItem
                .WithMany()  // One InventoryItem can be in many SaleItems
                .HasForeignKey(si => si.ItemId) // ItemId is the foreign key
                .OnDelete(DeleteBehavior.Restrict); // Optional: Prevent deletion of an InventoryItem if it's linked to a SaleItem

            // Configure Sale entity
           modelBuilder.Entity<Sale>()
                .HasMany(s => s.ItemsSold)
                .WithOne(si => si.Sale)  // Assuming there's a navigation property in SaleItem to Sale
                .HasForeignKey(si => si.ItemId);
        }
    }
}
