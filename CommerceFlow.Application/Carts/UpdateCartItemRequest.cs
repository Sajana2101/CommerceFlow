using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Carts
{
    public sealed record UpdateCartItemRequest(
        int Quantity);
}