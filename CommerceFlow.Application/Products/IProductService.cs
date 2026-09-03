using CommerceFlow.Application.Common;

namespace CommerceFlow.Application.Products
{
    public interface IProductService
    {
        Task<PagedResult<ProductResponse>> GetAsync(
            ProductQueryParameters parameters,
            CancellationToken cancellationToken = default);

        Task<ProductResponse?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<ProductResponse> CreateAsync(
            CreateProductRequest request,
            CancellationToken cancellationToken = default);

        Task<ProductResponse?> UpdateAsync(
            Guid id,
            UpdateProductRequest request,
            CancellationToken cancellationToken = default);

        Task<bool> DeactivateAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}