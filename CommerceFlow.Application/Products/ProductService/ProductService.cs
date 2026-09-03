using CommerceFlow.Application.Common;
using CommerceFlow.Domain.Entities;

namespace CommerceFlow.Application.Products
{
    public sealed class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PagedResult<ProductResponse>> GetAsync(
            ProductQueryParameters parameters,
            CancellationToken cancellationToken = default)
        {
            ValidateQueryParameters(parameters);

            var result = await _productRepository.GetPagedAsync(
                parameters,
                cancellationToken);

            var products = result.Items
                .Select(ToResponse)
                .ToArray();

            var totalPages = result.TotalCount == 0
                ? 0
                : (int)Math.Ceiling(
                    result.TotalCount / (double)parameters.PageSize);

            return new PagedResult<ProductResponse>(
                products,
                parameters.Page,
                parameters.PageSize,
                result.TotalCount,
                totalPages);
        }

        public async Task<ProductResponse?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(
                id,
                cancellationToken);

            return product is null
                ? null
                : ToResponse(product);
        }

        public async Task<ProductResponse> CreateAsync(
            CreateProductRequest request,
            CancellationToken cancellationToken = default)
        {
            var skuExists = await _productRepository.SkuExistsAsync(
                request.Sku,
                cancellationToken);

            if (skuExists)
                throw new InvalidOperationException(
                    $"A product with SKU '{request.Sku}' already exists.");

            var product = new Product(
                request.Name,
                request.Description,
                request.Sku,
                request.Price);

            await _productRepository.AddAsync(
                product,
                cancellationToken);

            return ToResponse(product);
        }

        public async Task<ProductResponse?> UpdateAsync(
            Guid id,
            UpdateProductRequest request,
            CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdForUpdateAsync(
                id,
                cancellationToken);

            if (product is null)
                return null;

            product.UpdateDetails(
                request.Name,
                request.Description,
                request.Price);

            await _productRepository.SaveChangesAsync(
                cancellationToken);

            return ToResponse(product);
        }

        public async Task<bool> DeactivateAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdForUpdateAsync(
                id,
                cancellationToken);

            if (product is null)
                return false;

            product.Deactivate();

            await _productRepository.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        private static void ValidateQueryParameters(
            ProductQueryParameters parameters)
        {
            if (parameters.Page < 1)
                throw new ArgumentException(
                    "Page must be at least 1.");

            if (parameters.PageSize < 1 ||
                parameters.PageSize > 100)
            {
                throw new ArgumentException(
                    "Page size must be between 1 and 100.");
            }

            if (parameters.MinPrice.HasValue &&
                parameters.MinPrice.Value < 0)
            {
                throw new ArgumentException(
                    "Minimum price cannot be negative.");
            }

            if (parameters.MaxPrice.HasValue &&
                parameters.MaxPrice.Value < 0)
            {
                throw new ArgumentException(
                    "Maximum price cannot be negative.");
            }

            if (parameters.MinPrice.HasValue &&
                parameters.MaxPrice.HasValue &&
                parameters.MinPrice.Value >
                parameters.MaxPrice.Value)
            {
                throw new ArgumentException(
                    "Minimum price cannot be greater than maximum price.");
            }

            var allowedSortFields = new[]
            {
                "name",
                "price",
                "created",
                "sku"
            };

            if (!allowedSortFields.Contains(
                parameters.SortBy.ToLowerInvariant()))
            {
                throw new ArgumentException(
                    "SortBy must be name, price, created, or sku.");
            }

            if (!string.Equals(
                    parameters.SortDirection,
                    "asc",
                    StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(
                    parameters.SortDirection,
                    "desc",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "SortDirection must be asc or desc.");
            }
        }

        private static ProductResponse ToResponse(Product product)
        {
            return new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Sku,
                product.Price,
                product.IsActive,
                product.CreatedAtUtc);
        }
    }
}