using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Domain.Entities
{
    public sealed class OrderItem
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public string Sku { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }

        public decimal LineTotal => UnitPrice * Quantity;

        private OrderItem()
        {
            ProductName = string.Empty;
            Sku = string.Empty;
        }

        public OrderItem(
            Guid orderId,
            Guid productId,
            string productName,
            string sku,
            decimal unitPrice,
            int quantity)
        {
            if (orderId == Guid.Empty)
                throw new ArgumentException(
                    "Order ID is required.",
                    nameof(orderId));

            if (productId == Guid.Empty)
                throw new ArgumentException(
                    "Product ID is required.",
                    nameof(productId));

            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException(
                    "Product name is required.",
                    nameof(productName));

            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException(
                    "SKU is required.",
                    nameof(sku));

            if (unitPrice < 0)
                throw new ArgumentException(
                    "Unit price cannot be negative.",
                    nameof(unitPrice));

            if (quantity < 1)
                throw new ArgumentException(
                    "Quantity must be at least 1.",
                    nameof(quantity));

            Id = Guid.NewGuid();
            OrderId = orderId;
            ProductId = productId;
            ProductName = productName.Trim();
            Sku = sku.Trim();
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }
}