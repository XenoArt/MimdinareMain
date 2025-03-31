// Controllers/PurchaseHistoryController.cs
using Microsoft.AspNetCore.Mvc;
using Mimdinare.Models;
using MimdinareMain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MimdinareMain.Controllers
{
    [Route("api/purchase-history")]
    [ApiController]
    public class PurchaseHistoryController : ControllerBase
    {
        private readonly IPurchaseHistoryService _service;

        public PurchaseHistoryController(IPurchaseHistoryService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<Purchase>> AddPurchase([FromBody] Purchase purchase)
        {
            var created = await _service.AddPurchaseAsync(purchase);
            return CreatedAtAction(nameof(GetPurchaseById), new { id = created.Id }, created);
        }

        [HttpGet]
        public async Task<ActionResult<List<Purchase>>> GetAllPurchases()
        {
            return Ok(await _service.GetAllPurchasesAsync());
        }

        [HttpGet("by-product/{productName}")]
        public async Task<ActionResult<List<Purchase>>> GetPurchasesByProductName(string productName)
        {
            return Ok(await _service.GetPurchasesByProductNameAsync(productName));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Purchase>> GetPurchaseById(int id)
        {
            var purchase = await _service.GetPurchaseByIdAsync(id);
            return purchase == null ? NotFound() : Ok(purchase);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Purchase>> UpdatePurchase(int id, [FromBody] Purchase purchase)
        {
            try
            {
                return Ok(await _service.UpdatePurchaseAsync(id, purchase));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            var success = await _service.DeletePurchaseAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}