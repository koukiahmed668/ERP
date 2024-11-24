using AutoMapper;
using ERP.api.inventory.dtos;
using ERP.api.sales.dtos;
using ERP.infrastructure.data;
using ERP.microservices.inventory.interfaces;
using ERP.microservices.sales.interfaces;
using ERP.models.inventory;
using ERP.models.sales;
using Microsoft.EntityFrameworkCore;

namespace ERP.microservices.sales.services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly IInventoryService _inventoryService;
        private readonly AppDbContext _context;  // Inject DbContext


        public SaleService(ISaleRepository saleRepository, IMapper mapper , IInventoryService inventoryService, AppDbContext context)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
            _inventoryService = inventoryService;
            _context = context;
        }

        public async Task<IEnumerable<SaleDto>> GetAllSalesAsync()
        {
            var sales = await _saleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<SaleDto> GetSaleByIdAsync(Guid id)
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            return _mapper.Map<SaleDto>(sale);
        }

        public async Task AddSaleAsync(SaleDto saleDto)
        {
            // Validate each item sold against the inventory
            foreach (var saleItemDto in saleDto.ItemsSold)  // First loop: saleItemDto
            {
                // Get the inventory item as DTO
                var inventoryItemDto = await _inventoryService.GetItemByIdAsync(saleItemDto.ItemId);

                // If the inventory item doesn't exist, throw an exception
                if (inventoryItemDto == null)
                {
                    throw new Exception($"Inventory item with ID {saleItemDto.ItemId} does not exist.");
                }

                // Optional: Check if sufficient quantity exists
                if (saleItemDto.Quantity > inventoryItemDto.Quantity)
                {
                    throw new Exception($"Not enough stock for item '{inventoryItemDto.Name}'. Available: {inventoryItemDto.Quantity}, Requested: {saleItemDto.Quantity}.");
                }

                // Map the SaleDto to a Sale entity
                var sale = _mapper.Map<Sale>(saleDto);  // Map SaleDto to Sale entity

                // Now, for each SaleItemDto, we need to map it to SaleItem and set the correct ItemId
                foreach (var innerSaleItemDto in saleDto.ItemsSold)  // Inner loop: renamed to innerSaleItemDto
                {
                    // Map the SaleItemDto to SaleItem
                    var saleItem = _mapper.Map<SaleItem>(innerSaleItemDto);

                    // Ensure that the SaleItem has the correct ItemId from the InventoryItem
                    var inventoryItem = await _inventoryService.GetItemByIdAsync(saleItem.ItemId);

                    if (inventoryItem == null)
                    {
                        throw new Exception($"Inventory item with ID {saleItem.ItemId} does not exist.");
                    }

                    // Optionally, check if sufficient stock is available
                    if (saleItem.Quantity > inventoryItem.Quantity)
                    {
                        throw new Exception($"Not enough stock for item '{inventoryItem.Name}'. Available: {inventoryItem.Quantity}, Requested: {saleItem.Quantity}.");
                    }

                    // Detach the existing SaleItem if it's already being tracked by the DbContext
                    var existingSaleItem = _context.SaleItems.Local.FirstOrDefault(si => si.ItemId == saleItem.ItemId);
                    if (existingSaleItem != null)
                    {
                        _context.Entry(existingSaleItem).State = EntityState.Detached;
                    }

                    // Add the SaleItem to the Sale's collection of items
                    sale.ItemsSold.Add(saleItem);  // Add the SaleItem to the Sale
                }

                // Add the Sale to the database
                await _saleRepository.AddAsync(sale);

                // Now, update the inventory quantities in the database
                foreach (var saleItem in saleDto.ItemsSold)
                {
                    // Fetch the inventory item again to ensure it's up-to-date
                    var inventoryItem = await _inventoryService.GetItemByIdAsync(saleItem.ItemId);

                    if (inventoryItem != null)
                    {
                        // Decrease the quantity in the inventory based on the sale quantity
                        inventoryItem.Quantity -= saleItem.Quantity;

                        // Now, update the inventory item in the database using the repository
                        await _inventoryService.UpdateItemAsync(_mapper.Map<InventoryItemDto>(inventoryItem));
                    }
                }
            }
        }













        public async Task UpdateSaleAsync(SaleDto saleDto)
        {
            var sale = _mapper.Map<Sale>(saleDto);
            await _saleRepository.UpdateAsync(sale);
        }

        public async Task DeleteSaleAsync(Guid id)
        {
            await _saleRepository.DeleteAsync(id);
        }
    }
}
