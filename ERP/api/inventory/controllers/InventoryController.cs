using ERP.api.inventory.dtos;
using ERP.infrastructure.cache;
using ERP.microservices.inventory.interfaces;
using ERP.models.inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ERP.api.inventory.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ICacheService _cacheService;


        public InventoryController(IInventoryService inventoryService, ICacheService cacheService)
        {
            _inventoryService = inventoryService;
            _cacheService = cacheService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var cacheKey = "inventory:all";
            var cachedItems = await _cacheService.GetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedItems))
            {
                try
                {
                    var items = JsonSerializer.Deserialize<List<InventoryItem>>(cachedItems, new JsonSerializerOptions
                    {
                        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                    });
                    return Ok(items);
                }
                catch (JsonException ex)
                {
                    // Log error and return meaningful response
                    Console.WriteLine($"Deserialization error: {ex.Message}");
                    return StatusCode(500, "Error deserializing cached data.");
                }
            }

            // Fetch from database if not cached
            var dbItems = await _inventoryService.GetAllItemsAsync();
            var serializedItems = JsonSerializer.Serialize(dbItems, new JsonSerializerOptions
            {
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            });

            await _cacheService.SetAsync(cacheKey, serializedItems, TimeSpan.FromMinutes(30)); // Cache for 30 minutes

            return Ok(dbItems);
        }




        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(Guid id)
        {
            var cacheKey = $"inventory:item:{id}";
            var cachedItem = await _cacheService.GetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedItem))
            {
                var item = JsonSerializer.Deserialize<InventoryItem>(cachedItem);
                return Ok(item);
            }

            // Fetch from database if not cached
            var dbItem = await _inventoryService.GetItemByIdAsync(id);
            if (dbItem == null) return NotFound();

            var serializedItem = JsonSerializer.Serialize(dbItem);
            await _cacheService.SetAsync(cacheKey, serializedItem, TimeSpan.FromMinutes(30)); // Cache for 30 minutes

            return Ok(dbItem);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] InventoryItemDto itemDto)
        {
            await _inventoryService.AddItemAsync(itemDto);

            // Invalidate "inventory:all" cache
            await _cacheService.RemoveAsync("inventory:all");

            return CreatedAtAction(nameof(GetItemById), new { id = itemDto.Id }, itemDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, [FromBody] InventoryItemDto itemDto)
        {
            if (id != itemDto.Id) return BadRequest();
            await _inventoryService.UpdateItemAsync(itemDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            await _inventoryService.DeleteItemAsync(id);

            // Invalidate both "inventory:all" and "inventory:item:{id}"
            await _cacheService.RemoveAsync("inventory:all");
            await _cacheService.RemoveAsync($"inventory:item:{id}");

            return NoContent();
        }

    }
}
