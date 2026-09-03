using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Payments
{
    public sealed record ProcessPaymentRequest(
        string IdempotencyKey,
        string PaymentMethodToken);
}