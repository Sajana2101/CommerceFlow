

using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommerceFlow.Infrastructure.Messaging
{
    public abstract class RabbitMqPaymentConsumerBase
        : BackgroundService
    {
        private readonly RabbitMqOptions _options;
        private readonly ILogger _logger;

        protected abstract string QueueName { get; }

        protected abstract string ConsumerName { get; }

        protected RabbitMqPaymentConsumerBase(
            IOptions<RabbitMqOptions> options,
            ILogger logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var factory =
                RabbitMqConnectionFactory.Create(_options);

            await using var connection =
                await factory.CreateConnectionAsync(
                    ConsumerName,
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

            await channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            await channel.QueueBindAsync(
                queue: QueueName,
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

                var json = Encoding.UTF8.GetString(body);

                try
                {
                    await HandleMessageAsync(
                        eventArgs.RoutingKey,
                        json,
                        stoppingToken);

                    await channel.BasicAckAsync(
                        eventArgs.DeliveryTag,
                        multiple: false,
                        cancellationToken: stoppingToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError(
                        exception,
                        "{ConsumerName} failed to process message {MessageId}.",
                        ConsumerName,
                        eventArgs.BasicProperties.MessageId);

                    await channel.BasicNackAsync(
                        eventArgs.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        cancellationToken: stoppingToken);
                }
            };

            await channel.BasicConsumeAsync(
                queue: QueueName,
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

        protected abstract Task HandleMessageAsync(
            string routingKey,
            string json,
            CancellationToken cancellationToken);
    }
}