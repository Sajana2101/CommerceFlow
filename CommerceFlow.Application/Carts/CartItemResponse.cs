using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Carts
{
    public sealed record CartItemResponse(
        Guid ProductId,
        string Name,
        string Sku,
        decimal UnitPrice,
        int Quantity,
        decimal LineTotal);
}