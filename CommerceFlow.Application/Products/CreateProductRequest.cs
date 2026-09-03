namespace CommerceFlow.Application.Products
{
    public sealed record CreateProductRequest(
        string Name,
        string Description,
        string Sku,
        decimal Price);
}
