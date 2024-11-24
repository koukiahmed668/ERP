using ERP.api.sales.dtos;

namespace ERP.microservices.sales.interfaces
{
    public interface ISaleService
    {
        Task<IEnumerable<SaleDto>> GetAllSalesAsync();
        Task<SaleDto> GetSaleByIdAsync(Guid id);
        Task AddSaleAsync(SaleDto saleDto);
        Task UpdateSaleAsync(SaleDto saleDto);
        Task DeleteSaleAsync(Guid id);
    }
}
