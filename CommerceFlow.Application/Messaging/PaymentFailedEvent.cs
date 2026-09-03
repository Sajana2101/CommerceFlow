using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Messaging
{
    public sealed record PaymentFailedEvent(
        Guid EventId,
        Guid PaymentId,
        Guid OrderId,
        Guid CustomerId,
        decimal Amount,
        string FailureReason,
        DateTime OccurredAtUtc);
}