using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Carts
{
    public sealed record CartResponse(
        Guid CustomerId,
        IReadOnlyCollection<CartItemResponse> Items,
        int TotalItems,
        decimal Subtotal);
}