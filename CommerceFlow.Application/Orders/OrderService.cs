using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Application.Carts;
using CommerceFlow.Application.Products;
using CommerceFlow.Domain.Entities;

namespace CommerceFlow.Application.Orders
{
    public sealed class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<OrderResponse> CheckoutAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var cart = await _cartRepository.GetAsync(
                customerId,
                cancellationToken);

            if (cart is null || cart.Items.Count == 0)
                throw new InvalidOperationException(
                    "Cart is empty.");

            var productIds = cart.Items
                .Select(item => item.ProductId)
                .ToArray();

            var products = await _productRepository.GetByIdsAsync(
                productIds,
                cancellationToken);

            var productMap = products.ToDictionary(
                product => product.Id);

            if (productMap.Count != productIds.Distinct().Count())
                throw new InvalidOperationException(
                    "One or more products in the cart are no longer available.");

            var order = new Order(customerId);

            foreach (var cartItem in cart.Items)
            {
                var product = productMap[cartItem.ProductId];

                order.AddItem(
                    product.Id,
                    product.Name,
                    product.Sku,
                    product.Price,
                    cartItem.Quantity);
            }

            var reserved =
    await _orderRepository.AddWithInventoryReservationAsync(
        order,
        cancellationToken);

            if (!reserved)
            {
                throw new InvalidOperationException(
                    "One or more products do not have enough stock.");
            }

            await _cartRepository.DeleteAsync(
                customerId,
                cancellationToken);

            return ToResponse(order);
        }

        public async Task<IReadOnlyCollection<OrderResponse>> GetOrdersAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(
                customerId,
                cancellationToken);

            return orders
                .Select(ToResponse)
                .ToArray();
        }

        public async Task<OrderResponse?> GetOrderAsync(
            Guid customerId,
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(
                orderId,
                customerId,
                cancellationToken);

            return order is null
                ? null
                : ToResponse(order);
        }

        private static OrderResponse ToResponse(Order order)
        {
            var items = order.Items
                .Select(item =>
                    new OrderItemResponse(
                        item.ProductId,
                        item.ProductName,
                        item.Sku,
                        item.UnitPrice,
                        item.Quantity,
                        item.LineTotal))
                .ToArray();

            return new OrderResponse(
                order.Id,
                order.OrderNumber,
                order.CustomerId,
                order.Status.ToString(),
                order.TotalAmount,
                order.CreatedAtUtc,
                items);
        }
    }
}