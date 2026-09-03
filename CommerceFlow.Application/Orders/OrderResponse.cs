namespace CommerceFlow.Application.Orders
{
    public sealed record OrderResponse(
        Guid Id,
        string OrderNumber,
        Guid CustomerId,
        string Status,
        decimal TotalAmount,
        DateTime CreatedAtUtc,
        IReadOnlyCollection<OrderItemResponse> Items);
}