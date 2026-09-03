using System;
using System.Collections.Generic;
using System.Text;

using CommerceFlow.Domain.Entities;

namespace CommerceFlow.Application.Payments
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdempotencyKeyAsync(
            string idempotencyKey,
            Guid customerId,
            CancellationToken cancellationToken = default);

        Task<Payment> RecordResultAsync(
            Guid orderId,
            Guid customerId,
            string idempotencyKey,
            bool succeeded,
            string? failureReason,
            CancellationToken cancellationToken = default);
    }
}