using System.Text.Json;
using CommerceFlow.Application.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CommerceFlow.Infrastructure.Messaging
{
    public sealed class PaymentAnalyticsConsumer
        : RabbitMqPaymentConsumerBase
    {
        private readonly ILogger<PaymentAnalyticsConsumer>
            _logger;

        protected override string QueueName =>
            "commerceflow.analytics.payment";

        protected override string ConsumerName =>
            "CommerceFlow-AnalyticsConsumer";

        public PaymentAnalyticsConsumer(
            IOptions<RabbitMqOptions> options,
            ILogger<PaymentAnalyticsConsumer> logger)
            : base(options, logger)
        {
            _logger = logger;
        }

        protected override Task HandleMessageAsync(
            string routingKey,
            string json,
            CancellationToken cancellationToken)
        {
            switch (routingKey)
            {
                case MessageRoutingKeys.PaymentSucceeded:
                    {
                        var payment =
                            JsonSerializer.Deserialize<
                                PaymentSucceededEvent>(json)
                            ?? throw new InvalidOperationException(
                                "Invalid payment succeeded event.");

                        _logger.LogInformation(
                            "[AnalyticsConsumer] Successful payment {PaymentId}. Amount {Amount}.",
                            payment.PaymentId,
                            payment.Amount);

                        break;
                    }

                case MessageRoutingKeys.PaymentFailed:
                    {
                        var payment =
                            JsonSerializer.Deserialize<
                                PaymentFailedEvent>(json)
                            ?? throw new InvalidOperationException(
                                "Invalid payment failed event.");

                        _logger.LogInformation(
                            "[AnalyticsConsumer] Failed payment {PaymentId}. Amount {Amount}.",
                            payment.PaymentId,
                            payment.Amount);

                        break;
                    }
            }

            return Task.CompletedTask;
        }
    }
}