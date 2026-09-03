using System;
using System.Collections.Generic;
using System.Text;
namespace CommerceFlow.Application.Orders
{
    public sealed record OrderItemResponse(
        Guid ProductId,
        string ProductName,
        string Sku,
        decimal UnitPrice,
        int Quantity,
        decimal LineTotal);
}