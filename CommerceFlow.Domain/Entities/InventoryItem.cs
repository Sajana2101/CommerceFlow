using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Domain.Entities
{
    public sealed class InventoryItem
    {
        public Guid ProductId { get; private set; }
        public int AvailableQuantity { get; private set; }
        public int ReservedQuantity { get; private set; }

        public int TotalQuantity =>
            AvailableQuantity + ReservedQuantity;

        private InventoryItem()
        {
        }

        public InventoryItem(Guid productId)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException(
                    "Product ID is required.",
                    nameof(productId));

            ProductId = productId;
            AvailableQuantity = 0;
            ReservedQuantity = 0;
        }

        public void SetAvailableQuantity(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException(
                    "Available quantity cannot be negative.",
                    nameof(quantity));

            AvailableQuantity = quantity;
        }

        public void ReleaseReservation(int quantity)
        {
            if (quantity < 1)
                throw new ArgumentException(
                    "Quantity must be at least 1.",
                    nameof(quantity));

            if (quantity > ReservedQuantity)
                throw new InvalidOperationException(
                    "Cannot release more stock than is reserved.");

            ReservedQuantity -= quantity;
            AvailableQuantity += quantity;
        }

        public void CompleteReservation(int quantity)
        {
            if (quantity < 1)
                throw new ArgumentException(
                    "Quantity must be at least 1.",
                    nameof(quantity));

            if (quantity > ReservedQuantity)
                throw new InvalidOperationException(
                    "Cannot complete more stock than is reserved.");

            ReservedQuantity -= quantity;
        }
    }
}