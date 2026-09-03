using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Payments
{
    public sealed record PaymentResponse(
        Guid Id,
        Guid OrderId,
        decimal Amount,
        string Status,
        string OrderStatus,
        string? FailureReason,
        DateTime CreatedAtUtc,
        DateTime? ProcessedAtUtc);
}