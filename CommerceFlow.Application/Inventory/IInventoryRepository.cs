using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Domain.Entities;

namespace CommerceFlow.Application.Inventory
{
    public interface IInventoryRepository
    {
        Task<InventoryItem?> GetByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default);

        Task<InventoryItem?> GetByProductIdForUpdateAsync(
            Guid productId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            InventoryItem inventory,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}