using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Inventory
{
    public sealed record InventoryResponse(
        Guid ProductId,
        int AvailableQuantity,
        int ReservedQuantity,
        int TotalQuantity);
}