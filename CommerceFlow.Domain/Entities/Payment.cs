using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Domain.Enums;

namespace CommerceFlow.Domain.Entities
{
    public sealed class Payment
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid CustomerId { get; private set; }
        public decimal Amount { get; private set; }
        public string IdempotencyKey { get; private set; }
        public PaymentStatus Status { get; private set; }
        public string? FailureReason { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? ProcessedAtUtc { get; private set; }

        private Payment()
        {
            IdempotencyKey = string.Empty;
        }

        public Payment(
            Guid orderId,
            Guid customerId,
            decimal amount,
            string idempotencyKey)
        {
            if (orderId == Guid.Empty)
                throw new ArgumentException(
                    "Order ID is required.",
                    nameof(orderId));

            if (customerId == Guid.Empty)
                throw new ArgumentException(
                    "Customer ID is required.",
                    nameof(customerId));

            if (amount <= 0)
                throw new ArgumentException(
                    "Payment amount must be greater than zero.",
                    nameof(amount));

            if (string.IsNullOrWhiteSpace(idempotencyKey))
                throw new ArgumentException(
                    "Idempotency key is required.",
                    nameof(idempotencyKey));

            Id = Guid.NewGuid();
            OrderId = orderId;
            CustomerId = customerId;
            Amount = amount;
            IdempotencyKey = idempotencyKey.Trim();
            Status = PaymentStatus.Pending;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public void MarkSucceeded()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException(
                    "Payment has already been processed.");

            Status = PaymentStatus.Succeeded;
            FailureReason = null;
            ProcessedAtUtc = DateTime.UtcNow;
        }

        public void MarkFailed(string reason)
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException(
                    "Payment has already been processed.");

            Status = PaymentStatus.Failed;
            FailureReason = reason;
            ProcessedAtUtc = DateTime.UtcNow;
        }
    }
}