using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Payments
{
    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessAsync(
            Guid customerId,
            Guid orderId,
            ProcessPaymentRequest request,
            CancellationToken cancellationToken = default);
    }
}