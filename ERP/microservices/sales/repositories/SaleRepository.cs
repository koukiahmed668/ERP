
using ERP.infrastructure.data;
using ERP.microservices.sales.interfaces;
using ERP.models.sales;
using Microsoft.EntityFrameworkCore;
namespace ERP.microservices.sales.repositories

{
    public class SaleRepository : ISaleRepository 
    {

        private readonly AppDbContext _context;

        public SaleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sale>> GetAllAsync() => await _context.Sales.Include(s => s.ItemsSold).ToListAsync();

        public async Task<Sale?> GetByIdAsync(Guid id) => await _context.Sales.Include(s => s.ItemsSold).FirstOrDefaultAsync(s => s.Id == id);

        public async Task AddAsync(Sale sale)
        {
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Sale sale)
        {
            _context.Sales.Update(sale);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var sale = await GetByIdAsync(id);
            if (sale != null)
            {
                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
            }
        }

    }
}
