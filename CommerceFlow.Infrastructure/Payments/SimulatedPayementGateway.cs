using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Application.Payments;

namespace CommerceFlow.Infrastructure.Payments
{
    public sealed class SimulatedPaymentGateway
        : IPaymentGateway
    {
        public Task<PaymentGatewayResult> ProcessAsync(
            decimal amount,
            string paymentMethodToken,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return paymentMethodToken switch
            {
                "tok_success" =>
                    Task.FromResult(
                        new PaymentGatewayResult(
                            true,
                            null)),

                "tok_fail" =>
                    Task.FromResult(
                        new PaymentGatewayResult(
                            false,
                            "Payment was declined by the simulated payment provider.")),

                _ => throw new ArgumentException(
                    "Invalid payment method token.")
            };
        }
    }
}
