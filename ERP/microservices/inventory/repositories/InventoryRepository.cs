using ERP.infrastructure.data;
using ERP.microservices.inventory.interfaces;
using ERP.models.inventory;
using Microsoft.EntityFrameworkCore;
using System;


namespace ERP.microservices.inventory.repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryItem>> GetAllAsync()
        {
            return await _context.InventoryItems.ToListAsync();
        }

        // This method returns IQueryable so you can apply AsNoTracking if needed
        public IQueryable<InventoryItem> GetByIdQuery(Guid id)
        {
            return _context.InventoryItems.Where(x => x.Id == id);  // Returns IQueryable
        }

        // GetByIdAsync now uses the queryable and AsNoTracking in the service
        public async Task<InventoryItem> GetByIdAsync(Guid id)
        {
            return await _context.InventoryItems.FindAsync(id); // Using FindAsync for a single entity
        }

        public async Task AddAsync(InventoryItem item)
        {
            await _context.InventoryItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(InventoryItem item)
        {
            _context.InventoryItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item != null)
            {
                _context.InventoryItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
