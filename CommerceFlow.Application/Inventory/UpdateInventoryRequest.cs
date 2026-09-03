using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Inventory
{
    public sealed record UpdateInventoryRequest(
        int AvailableQuantity);
}