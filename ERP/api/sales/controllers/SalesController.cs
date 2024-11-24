using ERP.api.sales.dtos;
using ERP.microservices.sales.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.api.sales.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSales() => Ok(await _saleService.GetAllSalesAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSaleById(Guid id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpPost]
        public async Task<IActionResult> AddSale([FromBody] SaleDto saleDto)
        {
            await _saleService.AddSaleAsync(saleDto);
            return CreatedAtAction(nameof(GetSaleById), new { id = saleDto.Id }, saleDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSale(Guid id, [FromBody] SaleDto saleDto)
        {
            if (id != saleDto.Id) return BadRequest();
            await _saleService.UpdateSaleAsync(saleDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(Guid id)
        {
            await _saleService.DeleteSaleAsync(id);
            return NoContent();
        }
    }
}
