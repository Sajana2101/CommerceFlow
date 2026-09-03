using CommerceFlow.Application.Common;
using CommerceFlow.Application.Products;
using Microsoft.AspNetCore.Mvc;

namespace CommerceFlow.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public sealed class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductResponse>>> GetAll(
            [FromQuery] ProductQueryParameters parameters,
            CancellationToken cancellationToken)
        {
            try
            {
                var products = await _productService.GetAsync(
                    parameters,
                    cancellationToken);

                return Ok(products);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new
                {
                    message = exception.Message
                });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductResponse>> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(
                id,
                cancellationToken);

            if (product is null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponse>> Create(
            CreateProductRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productService.CreateAsync(
                    request,
                    cancellationToken);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = product.Id },
                    product);
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new
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

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProductResponse>> Update(
            Guid id,
            UpdateProductRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productService.UpdateAsync(
                    id,
                    request,
                    cancellationToken);

                if (product is null)
                    return NotFound();

                return Ok(product);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new
                {
                    message = exception.Message
                });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            var deleted = await _productService.DeactivateAsync(
                id,
                cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}