using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Inventory
{
    public interface IInventoryService
    {
        Task<InventoryResponse?> GetAsync(
            Guid productId,
            CancellationToken cancellationToken = default);

        Task<InventoryResponse> SetAvailableQuantityAsync(
            Guid productId,
            UpdateInventoryRequest request,
            CancellationToken cancellationToken = default);
    }
}
