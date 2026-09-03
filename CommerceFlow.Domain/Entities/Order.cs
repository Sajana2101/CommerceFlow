using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Domain.Enums;

namespace CommerceFlow.Domain.Entities
{
    public sealed class Order
    {
        private readonly List<OrderItem> _items = new();

        public Guid Id { get; private set; }
        public string OrderNumber { get; private set; }
        public Guid CustomerId { get; private set; }
        public OrderStatus Status { get; private set; }
        public decimal TotalAmount { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        public IReadOnlyCollection<OrderItem> Items => _items;

        private Order()
        {
            OrderNumber = string.Empty;
        }

        public Order(Guid customerId)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException(
                    "Customer ID is required.",
                    nameof(customerId));

            Id = Guid.NewGuid();
            OrderNumber = GenerateOrderNumber();
            CustomerId = customerId;
            Status = OrderStatus.PendingPayment;
            TotalAmount = 0;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public void AddItem(
            Guid productId,
            string productName,
            string sku,
            decimal unitPrice,
            int quantity)
        {
            var item = new OrderItem(
                Id,
                productId,
                productName,
                sku,
                unitPrice,
                quantity);

            _items.Add(item);

            TotalAmount += item.LineTotal;
        }

        public void MarkPaid()
        {
            if (Status != OrderStatus.PendingPayment)
                throw new InvalidOperationException(
                    "Order is not awaiting payment.");

            Status = OrderStatus.Paid;
        }

        public void MarkPaymentFailed()
        {
            if (Status != OrderStatus.PendingPayment)
                throw new InvalidOperationException(
                    "Order is not awaiting payment.");

            Status = OrderStatus.PaymentFailed;
        }

        private static string GenerateOrderNumber()
        {
            return $"CF-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        }
    }
}
