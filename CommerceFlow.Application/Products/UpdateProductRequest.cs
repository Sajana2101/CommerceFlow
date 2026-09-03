namespace CommerceFlow.Application.Products
{
    public sealed record UpdateProductRequest(
        string Name,
        string Description,
        decimal Price);
}