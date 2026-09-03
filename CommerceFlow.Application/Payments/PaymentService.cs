using CommerceFlow.Application.Messaging;
using CommerceFlow.Application.Orders;
using CommerceFlow.Domain.Entities;
using CommerceFlow.Domain.Enums;

namespace CommerceFlow.Application.Payments
{
    public sealed class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IMessagePublisher _messagePublisher;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository,
            IPaymentGateway paymentGateway,
            IMessagePublisher messagePublisher)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _paymentGateway = paymentGateway;
            _messagePublisher = messagePublisher;
        }

        public async Task<PaymentResponse> ProcessAsync(
            Guid customerId,
            Guid orderId,
            ProcessPaymentRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(
                request.IdempotencyKey))
            {
                throw new ArgumentException(
                    "Idempotency key is required.");
            }

            if (string.IsNullOrWhiteSpace(
                request.PaymentMethodToken))
            {
                throw new ArgumentException(
                    "Payment method token is required.");
            }

            var existingPayment =
                await _paymentRepository
                    .GetByIdempotencyKeyAsync(
                        request.IdempotencyKey,
                        customerId,
                        cancellationToken);

            if (existingPayment is not null)
            {
                if (existingPayment.OrderId != orderId)
                {
                    throw new InvalidOperationException(
                        "This idempotency key has already been used for another order.");
                }

                var existingOrder =
                    await _orderRepository.GetByIdAsync(
                        orderId,
                        customerId,
                        cancellationToken);

                return ToResponse(
                    existingPayment,
                    existingOrder?.Status.ToString()
                        ?? "Unknown");
            }

            var order =
                await _orderRepository.GetByIdAsync(
                    orderId,
                    customerId,
                    cancellationToken);

            if (order is null)
                throw new KeyNotFoundException(
                    "Order not found.");

            if (order.Status !=
                OrderStatus.PendingPayment)
            {
                throw new InvalidOperationException(
                    "Order is not awaiting payment.");
            }

            var gatewayResult =
                await _paymentGateway.ProcessAsync(
                    order.TotalAmount,
                    request.PaymentMethodToken,
                    cancellationToken);

            var payment =
                await _paymentRepository.RecordResultAsync(
                    orderId,
                    customerId,
                    request.IdempotencyKey,
                    gatewayResult.Succeeded,
                    gatewayResult.FailureReason,
                    cancellationToken);

            if (gatewayResult.Succeeded)
            {
                var paymentEvent =
                    new PaymentSucceededEvent(
                        Guid.NewGuid(),
                        payment.Id,
                        payment.OrderId,
                        customerId,
                        payment.Amount,
                        payment.ProcessedAtUtc
                            ?? DateTime.UtcNow);

                await _messagePublisher.PublishAsync(
                    MessageRoutingKeys.PaymentSucceeded,
                    paymentEvent,
                    cancellationToken);
            }
            else
            {
                var paymentEvent =
                    new PaymentFailedEvent(
                        Guid.NewGuid(),
                        payment.Id,
                        payment.OrderId,
                        customerId,
                        payment.Amount,
                        payment.FailureReason
                            ?? "Payment failed.",
                        payment.ProcessedAtUtc
                            ?? DateTime.UtcNow);

                await _messagePublisher.PublishAsync(
                    MessageRoutingKeys.PaymentFailed,
                    paymentEvent,
                    cancellationToken);
            }

            var orderStatus =
                gatewayResult.Succeeded
                    ? OrderStatus.Paid.ToString()
                    : OrderStatus.PaymentFailed.ToString();

            return ToResponse(
                payment,
                orderStatus);
        }

        private static PaymentResponse ToResponse(
            Payment payment,
            string orderStatus)
        {
            return new PaymentResponse(
                payment.Id,
                payment.OrderId,
                payment.Amount,
                payment.Status.ToString(),
                orderStatus,
                payment.FailureReason,
                payment.CreatedAtUtc,
                payment.ProcessedAtUtc);
        }
    }
}