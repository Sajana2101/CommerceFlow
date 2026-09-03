using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Messaging
{
    public sealed record PaymentSucceededEvent(
        Guid EventId,
        Guid PaymentId,
        Guid OrderId,
        Guid CustomerId,
        decimal Amount,
        DateTime OccurredAtUtc);
}