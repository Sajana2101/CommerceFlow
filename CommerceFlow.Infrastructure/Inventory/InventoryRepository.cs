using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Application.Inventory;
using CommerceFlow.Domain.Entities;
using CommerceFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CommerceFlow.Infrastructure.Inventory
{
    public sealed class InventoryRepository
        : IInventoryRepository
    {
        private readonly CommerceFlowDbContext _dbContext;

        public InventoryRepository(
            CommerceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<InventoryItem?> GetByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.InventoryItems
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    inventory =>
                        inventory.ProductId == productId,
                    cancellationToken);
        }

        public async Task<InventoryItem?> GetByProductIdForUpdateAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.InventoryItems
                .FirstOrDefaultAsync(
                    inventory =>
                        inventory.ProductId == productId,
                    cancellationToken);
        }

        public async Task AddAsync(
            InventoryItem inventory,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.InventoryItems.AddAsync(
                inventory,
                cancellationToken);

            await _dbContext.SaveChangesAsync(
                cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(
                cancellationToken);
        }
    }
}