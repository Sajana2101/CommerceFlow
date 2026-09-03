using Microsoft.AspNetCore.Mvc;
using CommerceFlow.Application.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommerceFlow.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/inventory")]
    public sealed class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(
            IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("{productId:guid}")]
        public async Task<ActionResult<InventoryResponse>> Get(
            Guid productId,
            CancellationToken cancellationToken)
        {
            var inventory =
                await _inventoryService.GetAsync(
                    productId,
                    cancellationToken);

            if (inventory is null)
                return NotFound();

            return Ok(inventory);
        }

        [HttpPut("{productId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InventoryResponse>> Update(
            Guid productId,
            UpdateInventoryRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var inventory =
                    await _inventoryService.SetAvailableQuantityAsync(
                        productId,
                        request,
                        cancellationToken);

                return Ok(inventory);
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(new
                {
                    message = exception.Message
                });
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new
                {
                    message = exception.Message
                });
            }
        }
    }
}