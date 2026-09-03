using System.Security.Claims;
using CommerceFlow.Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommerceFlow.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/account")]
    public sealed class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerResponse>> GetAccount(
            CancellationToken cancellationToken)
        {
            var customerIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(
                customerIdValue,
                out var customerId))
            {
                return Unauthorized();
            }

            var customer = await _authService.GetCustomerAsync(
                customerId,
                cancellationToken);

            if (customer is null)
                return NotFound();

            return Ok(customer);
        }
    }
}