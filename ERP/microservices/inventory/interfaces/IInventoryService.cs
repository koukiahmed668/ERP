using ERP.api.inventory.dtos;
using ERP.models.inventory;

namespace ERP.microservices.inventory.interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryItemDto>> GetAllItemsAsync();
        Task<InventoryItemDto> GetItemByIdAsync(Guid id);
        Task AddItemAsync(InventoryItemDto item);
        Task UpdateItemAsync(InventoryItemDto item);
        Task DeleteItemAsync(Guid id);
    }
}
