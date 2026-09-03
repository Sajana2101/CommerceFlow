using CommerceFlow.Application.Products;
using CommerceFlow.Domain.Entities;
using CommerceFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using CommerceFlow.Domain.Entities;

namespace CommerceFlow.Infrastructure.Products
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly CommerceFlowDbContext _dbContext;

        public async Task<IReadOnlyCollection<Product>> GetByIdsAsync(
    IEnumerable<Guid> ids,
    CancellationToken cancellationToken = default)
        {
            var productIds = ids.Distinct().ToArray();

            return await _dbContext.Products
                .AsNoTracking()
                .Where(product =>
                    productIds.Contains(product.Id) &&
                    product.IsActive)
                .ToArrayAsync(cancellationToken);
        }
        public ProductRepository(CommerceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(IReadOnlyCollection<Product> Items, int TotalCount)> GetPagedAsync(
            ProductQueryParameters parameters,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Products
                .AsNoTracking()
                .Where(product => product.IsActive);

            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var search = parameters.Search.Trim();

                query = query.Where(product =>
                    product.Name.Contains(search) ||
                    product.Description.Contains(search) ||
                    product.Sku.Contains(search));
            }

            if (parameters.MinPrice.HasValue)
            {
                query = query.Where(product =>
                    product.Price >= parameters.MinPrice.Value);
            }

            if (parameters.MaxPrice.HasValue)
            {
                query = query.Where(product =>
                    product.Price <= parameters.MaxPrice.Value);
            }

            var totalCount = await query.CountAsync(
                cancellationToken);

            var descending = string.Equals(
                parameters.SortDirection,
                "desc",
                StringComparison.OrdinalIgnoreCase);

            query = parameters.SortBy.ToLowerInvariant() switch
            {
                "price" => descending
                    ? query.OrderByDescending(product => product.Price)
                    : query.OrderBy(product => product.Price),

                "created" => descending
                    ? query.OrderByDescending(product => product.CreatedAtUtc)
                    : query.OrderBy(product => product.CreatedAtUtc),

                "sku" => descending
                    ? query.OrderByDescending(product => product.Sku)
                    : query.OrderBy(product => product.Sku),

                _ => descending
                    ? query.OrderByDescending(product => product.Name)
                    : query.OrderBy(product => product.Name)
            };

            var products = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToArrayAsync(cancellationToken);

            return (products, totalCount);
        }

        public async Task<Product?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    product =>
                        product.Id == id &&
                        product.IsActive,
                    cancellationToken);
        }

        public async Task<Product?> GetByIdForUpdateAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .FirstOrDefaultAsync(
                    product =>
                        product.Id == id &&
                        product.IsActive,
                    cancellationToken);
        }

        public async Task<bool> SkuExistsAsync(
            string sku,
            CancellationToken cancellationToken = default)
        {
            var normalizedSku = sku.Trim().ToUpperInvariant();

            return await _dbContext.Products
                .AnyAsync(
                    product => product.Sku == normalizedSku,
                    cancellationToken);
        }

        public async Task AddAsync(
    Product product,
    CancellationToken cancellationToken = default)
        {
            await _dbContext.Products.AddAsync(
                product,
                cancellationToken);

            var inventory = new InventoryItem(
                product.Id);

            await _dbContext.InventoryItems.AddAsync(
                inventory,
                cancellationToken);

            await _dbContext.SaveChangesAsync(
                cancellationToken);
        }
        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(
                cancellationToken);
        }
    }
}