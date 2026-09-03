using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Domain.Entities;
using CommerceFlow.Domain.Entities;

namespace CommerceFlow.Application.Orders
{
    public interface IOrderRepository
    {
        Task<bool> AddWithInventoryReservationAsync(
            Order order,
            CancellationToken cancellationToken = default);

        Task<Order?> GetByIdAsync(
            Guid orderId,
            Guid customerId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<Order>> GetByCustomerIdAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);
    }
}