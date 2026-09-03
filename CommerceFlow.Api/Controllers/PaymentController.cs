using System.Security.Claims;
using CommerceFlow.Application.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommerceFlow.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/orders/{orderId:guid}/payment")]
    public sealed class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(
            IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<ActionResult<PaymentResponse>> Process(
            Guid orderId,
            ProcessPaymentRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var customerId = GetCustomerId();

                var payment =
                    await _paymentService.ProcessAsync(
                        customerId,
                        orderId,
                        request,
                        cancellationToken);

                return Ok(payment);
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
                return Conflict(new
                {
                    message = exception.Message
                });
            }
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