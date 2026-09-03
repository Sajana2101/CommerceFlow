using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Application.Payments;
using CommerceFlow.Domain.Entities;
using CommerceFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CommerceFlow.Infrastructure.Payments
{
    public sealed class PaymentRepository
        : IPaymentRepository
    {
        private readonly CommerceFlowDbContext _dbContext;

        public PaymentRepository(
            CommerceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Payment?> GetByIdempotencyKeyAsync(
            string idempotencyKey,
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    payment =>
                        payment.IdempotencyKey == idempotencyKey &&
                        payment.CustomerId == customerId,
                    cancellationToken);
        }

        public async Task<Payment> RecordResultAsync(
            Guid orderId,
            Guid customerId,
            string idempotencyKey,
            bool succeeded,
            string? failureReason,
            CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync(
                    cancellationToken);

            var existing =
                await _dbContext.Payments
                    .FirstOrDefaultAsync(
                        payment =>
                            payment.IdempotencyKey ==
                            idempotencyKey,
                        cancellationToken);

            if (existing is not null)
            {
                if (existing.CustomerId != customerId ||
                    existing.OrderId != orderId)
                {
                    throw new InvalidOperationException(
                        "This idempotency key has already been used.");
                }

                await transaction.CommitAsync(
                    cancellationToken);

                return existing;
            }

            var order = await _dbContext.Orders
                .Include(order => order.Items)
                .FirstOrDefaultAsync(
                    order =>
                        order.Id == orderId &&
                        order.CustomerId == customerId,
                    cancellationToken);

            if (order is null)
                throw new KeyNotFoundException(
                    "Order not found.");

            if (order.Status != Domain.Enums.OrderStatus.PendingPayment)
                throw new InvalidOperationException(
                    "Order is not awaiting payment.");

            var payment = new Payment(
                order.Id,
                customerId,
                order.TotalAmount,
                idempotencyKey);

            foreach (var item in order.Items)
            {
                var inventory =
                    await _dbContext.InventoryItems
                        .FirstOrDefaultAsync(
                            inventory =>
                                inventory.ProductId ==
                                item.ProductId,
                            cancellationToken);

                if (inventory is null)
                    throw new InvalidOperationException(
                        $"Inventory record was not found for product {item.ProductId}.");

                if (succeeded)
                {
                    inventory.CompleteReservation(
                        item.Quantity);
                }
                else
                {
                    inventory.ReleaseReservation(
                        item.Quantity);
                }
            }

            if (succeeded)
            {
                payment.MarkSucceeded();
                order.MarkPaid();
            }
            else
            {
                payment.MarkFailed(
                    failureReason ??
                    "Payment failed.");

                order.MarkPaymentFailed();
            }

            await _dbContext.Payments.AddAsync(
                payment,
                cancellationToken);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return payment;
        }
    }
}