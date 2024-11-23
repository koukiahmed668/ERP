using AutoMapper;
using ERP.api.inventory.dtos;
using ERP.infrastructure.Mail;
using ERP.microservices.inventory.interfaces;
using ERP.models.inventory;

namespace ERP.microservices.inventory.services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;  // Using AutoMapper for mapping
        private readonly IEmailService _emailService;


        public InventoryService(IInventoryRepository inventoryRepository, IMapper mapper, IEmailService emailService)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
            _emailService = emailService;
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
            // Check for low stock
            if (item.Quantity <= item.LowStockThreshold)
            {
                await _emailService.SendEmailAsync(
                    "koukiahmed668@gmail.com",
                    "Low Stock Alert",
                    $"Item '{item.Name}' is running low with only {item.Quantity} left in stock."
                );
            }
        }

        // Update an inventory item
        public async Task UpdateItemAsync(InventoryItemDto itemDto)
        {
            var item = _mapper.Map<InventoryItem>(itemDto);
            await _inventoryRepository.UpdateAsync(item);

            // Check for low stock
            if (item.Quantity <= item.LowStockThreshold)
            {
                await _emailService.SendEmailAsync(
                    "koukiahmed668@gmail.com",
                    "Low Stock Alert",
                    $"Item '{item.Name}' is running low with only {item.Quantity} left in stock."
                );
            }
        }

        // Delete an inventory item
        public async Task DeleteItemAsync(Guid id)
        {
            await _inventoryRepository.DeleteAsync(id);
        }

        // Adjust stock due to sales
        public async Task AdjustStockAsync(Guid id, int quantitySold)
        {
            var item = await _inventoryRepository.GetByIdAsync(id);
            if (item == null) throw new Exception("Item not found");

            item.Quantity -= quantitySold;
            await _inventoryRepository.UpdateAsync(item);

            // Notify if stock is low
            if (item.Quantity <= item.LowStockThreshold)
            {
                await _emailService.SendEmailAsync(
                    "admin@example.com",
                    "Low Stock Alert",
                    $"Item '{item.Name}' is running low with only {item.Quantity} left in stock."
                );
            }
        }
    }
}
