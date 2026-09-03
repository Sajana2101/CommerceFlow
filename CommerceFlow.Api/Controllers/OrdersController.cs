using System.Security.Claims;
using CommerceFlow.Application.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommerceFlow.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/orders")]
    public sealed class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(
            IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<OrderResponse>> Checkout(
            CancellationToken cancellationToken)
        {
            try
            {
                var customerId = GetCustomerId();

                var order = await _orderService.CheckoutAsync(
                    customerId,
                    cancellationToken);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = order.Id },
                    order);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new
                {
                    message = exception.Message
                });
            }
        }

        [HttpGet]
        public async Task<
            ActionResult<IReadOnlyCollection<OrderResponse>>> GetAll(
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();

            var orders = await _orderService.GetOrdersAsync(
                customerId,
                cancellationToken);

            return Ok(orders);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderResponse>> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();

            var order = await _orderService.GetOrderAsync(
                customerId,
                id,
                cancellationToken);

            if (order is null)
                return NotFound();

            return Ok(order);
        }

        private Guid GetCustomerId()
        {
            var value = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(
                value,
                out var customerId))
            {
                throw new UnauthorizedAccessException();
            }

            return customerId;
        }
    }
}