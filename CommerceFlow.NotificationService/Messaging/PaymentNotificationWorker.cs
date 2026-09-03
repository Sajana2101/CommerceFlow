using System.Text;
using System.Text.Json;
using CommerceFlow.Application.Messaging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommerceFlow.NotificationService.Messaging
{
    public sealed class PaymentNotificationWorker
        : BackgroundService
    {
        private readonly RabbitMqOptions _options;
        private readonly ILogger<PaymentNotificationWorker> _logger;

        public PaymentNotificationWorker(
            IOptions<RabbitMqOptions> options,
            ILogger<PaymentNotificationWorker> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                AutomaticRecoveryEnabled = true
            };

            await using var connection =
                await factory.CreateConnectionAsync(
                    "CommerceFlow-NotificationService",
                    stoppingToken);

            await using var channel =
                await connection.CreateChannelAsync(
                    cancellationToken: stoppingToken);

            await channel.ExchangeDeclareAsync(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken);

            const string queueName =
                "commerceflow.notifications.payment";

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            await channel.QueueBindAsync(
                queue: queueName,
                exchange: _options.ExchangeName,
                routingKey: "payment.*",
                arguments: null,
                cancellationToken: stoppingToken);

            await channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 10,
                global: false,
                cancellationToken: stoppingToken);

            var consumer =
                new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();

                var json =
                    Encoding.UTF8.GetString(body);

                try
                {
                    await HandleMessageAsync(
                        eventArgs.RoutingKey,
                        json);

                    await channel.BasicAckAsync(
                        eventArgs.DeliveryTag,
                        multiple: false,
                        cancellationToken: stoppingToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError(
                        exception,
                        "Notification message failed.");

                    await channel.BasicNackAsync(
                        eventArgs.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        cancellationToken: stoppingToken);
                }
            };

            await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            try
            {
                await Task.Delay(
                    Timeout.Infinite,
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
            }
        }

        private Task HandleMessageAsync(
            string routingKey,
            string json)
        {
            switch (routingKey)
            {
                case MessageRoutingKeys.PaymentSucceeded:
                    {
                        var payment =
                            JsonSerializer.Deserialize<
                                PaymentSucceededEvent>(json);

                        if (payment is null)
                            throw new InvalidOperationException(
                                "Invalid PaymentSucceededEvent.");

                        _logger.LogInformation(
                            "Sending payment success notification for order {OrderId}.",
                            payment.OrderId);

                        break;
                    }

                case MessageRoutingKeys.PaymentFailed:
                    {
                        var payment =
                            JsonSerializer.Deserialize<
                                PaymentFailedEvent>(json);

                        if (payment is null)
                            throw new InvalidOperationException(
                                "Invalid PaymentFailedEvent.");

                        _logger.LogInformation(
                            "Sending payment failure notification for order {OrderId}.",
                            payment.OrderId);

                        break;
                    }
            }

            return Task.CompletedTask;
        }
    }
}