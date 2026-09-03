using CommerceFlow.Domain.Entities;

namespace CommerceFlow.Application.Products
{
    public interface IProductRepository
    {
        Task<IReadOnlyCollection<Product>> GetByIdsAsync(
    IEnumerable<Guid> ids,
    CancellationToken cancellationToken = default);

        Task<(IReadOnlyCollection<Product> Items, int TotalCount)> GetPagedAsync(
            ProductQueryParameters parameters,
            CancellationToken cancellationToken = default);

        Task<Product?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Product?> GetByIdForUpdateAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> SkuExistsAsync(
            string sku,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Product product,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}