using System.Security.Claims;
using CommerceFlow.Application.Carts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommerceFlow.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/cart")]
    public sealed class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(
            ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<CartResponse>> Get(
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();

            var cart = await _cartService.GetAsync(
                customerId,
                cancellationToken);

            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<ActionResult<CartResponse>> AddItem(
            AddCartItemRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var customerId = GetCustomerId();

                var cart = await _cartService.AddItemAsync(
                    customerId,
                    request,
                    cancellationToken);

                return Ok(cart);
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
            catch (InvalidOperationException exception)
            {
                return BadRequest(new
                {
                    message = exception.Message
                });
            }
        }

        [HttpPut("items/{productId:guid}")]
        public async Task<ActionResult<CartResponse>> UpdateItem(
            Guid productId,
            UpdateCartItemRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var customerId = GetCustomerId();

                var cart = await _cartService.UpdateItemAsync(
                    customerId,
                    productId,
                    request,
                    cancellationToken);

                if (cart is null)
                    return NotFound();

                return Ok(cart);
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

        [HttpDelete("items/{productId:guid}")]
        public async Task<IActionResult> RemoveItem(
            Guid productId,
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();

            var removed = await _cartService.RemoveItemAsync(
                customerId,
                productId,
                cancellationToken);

            if (!removed)
                return NotFound();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Clear(
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();

            await _cartService.ClearAsync(
                customerId,
                cancellationToken);

            return NoContent();
        }

        private Guid GetCustomerId()
        {
            var value = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(value, out var customerId))
                throw new UnauthorizedAccessException();

            return customerId;
        }
    }
}