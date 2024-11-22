using ERP.api.inventory.dtos;
using ERP.microservices.inventory.interfaces;
using ERP.models.inventory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP.api.inventory.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _inventoryService.GetAllItemsAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(Guid id)
        {
            var item = await _inventoryService.GetItemByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }
        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] InventoryItemDto itemDto)
        {
            await _inventoryService.AddItemAsync(itemDto);
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
            return NoContent();
        }
    }
}
