using CommerceFlow.Application.Orders;
using CommerceFlow.Domain.Entities;
using CommerceFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CommerceFlow.Infrastructure.Orders
{
    public sealed class OrderRepository : IOrderRepository
    {
        private readonly CommerceFlowDbContext _dbContext;

        public OrderRepository(
            CommerceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddWithInventoryReservationAsync(
            Order order,
            CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync(
                    cancellationToken);

            foreach (var item in order.Items)
            {
                var affectedRows =
                    await _dbContext.InventoryItems
                        .Where(inventory =>
                            inventory.ProductId == item.ProductId &&
                            inventory.AvailableQuantity >= item.Quantity)
                        .ExecuteUpdateAsync(
                            setters => setters
                                .SetProperty(
                                    inventory =>
                                        inventory.AvailableQuantity,
                                    inventory =>
                                        inventory.AvailableQuantity -
                                        item.Quantity)
                                .SetProperty(
                                    inventory =>
                                        inventory.ReservedQuantity,
                                    inventory =>
                                        inventory.ReservedQuantity +
                                        item.Quantity),
                            cancellationToken);

                if (affectedRows != 1)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return false;
                }
            }

            await _dbContext.Orders.AddAsync(
                order,
                cancellationToken);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return true;
        }

        public async Task<Order?> GetByIdAsync(
            Guid orderId,
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders
                .AsNoTracking()
                .Include(order => order.Items)
                .FirstOrDefaultAsync(
                    order =>
                        order.Id == orderId &&
                        order.CustomerId == customerId,
                    cancellationToken);
        }

        public async Task<IReadOnlyCollection<Order>> GetByCustomerIdAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders
                .AsNoTracking()
                .Include(order => order.Items)
                .Where(order =>
                    order.CustomerId == customerId)
                .OrderByDescending(order =>
                    order.CreatedAtUtc)
                .ToArrayAsync(cancellationToken);
        }
    }
}