using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Orders
{
    public interface IOrderService
    {
        Task<OrderResponse> CheckoutAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<OrderResponse>> GetOrdersAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);

        Task<OrderResponse?> GetOrderAsync(
            Guid customerId,
            Guid orderId,
            CancellationToken cancellationToken = default);
    }
}