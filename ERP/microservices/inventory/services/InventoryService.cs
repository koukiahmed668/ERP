using AutoMapper;
using ERP.api.inventory.dtos;
using ERP.microservices.inventory.interfaces;
using ERP.models.inventory;

namespace ERP.microservices.inventory.services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;  // Using AutoMapper for mapping

        public InventoryService(IInventoryRepository inventoryRepository, IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;  // Injecting AutoMapper
        }

        // Get all inventory items
        public async Task<IEnumerable<InventoryItemDto>> GetAllItemsAsync()
        {
            var items = await _inventoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<InventoryItemDto>>(items);  // Mapping entities to DTOs
        }

        // Get an inventory item by ID
        public async Task<InventoryItemDto> GetItemByIdAsync(Guid id)
        {
            var item = await _inventoryRepository.GetByIdAsync(id);
            return _mapper.Map<InventoryItemDto>(item);  // Mapping a single entity to DTO
        }

        // Add a new inventory item
        public async Task AddItemAsync(InventoryItemDto itemDto)
        {
            var item = _mapper.Map<InventoryItem>(itemDto);  // Mapping DTO to entity
            await _inventoryRepository.AddAsync(item);
        }

        // Update an inventory item
        public async Task UpdateItemAsync(InventoryItemDto itemDto)
        {
            var item = _mapper.Map<InventoryItem>(itemDto);  // Mapping DTO to entity
            await _inventoryRepository.UpdateAsync(item);
        }

        // Delete an inventory item
        public async Task DeleteItemAsync(Guid id)
        {
            await _inventoryRepository.DeleteAsync(id);
        }
    }
}
