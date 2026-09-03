namespace CommerceFlow.Application.Products
{
    public sealed record ProductResponse(
        Guid Id,
        string Name,
        string Description,
        string Sku,
        decimal Price,
        bool IsActive,
        DateTime CreatedAtUtc);
}