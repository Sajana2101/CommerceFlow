using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Payments
{
    public interface IPaymentGateway
    {
        Task<PaymentGatewayResult> ProcessAsync(
            decimal amount,
            string paymentMethodToken,
            CancellationToken cancellationToken = default);
    }
}