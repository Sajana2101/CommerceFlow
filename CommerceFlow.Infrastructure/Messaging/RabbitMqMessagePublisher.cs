using System.Text.Json;
using CommerceFlow.Application.Messaging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace CommerceFlow.Infrastructure.Messaging
{
    public sealed class RabbitMqMessagePublisher
        : IMessagePublisher
    {
        private readonly RabbitMqOptions _options;

        public RabbitMqMessagePublisher(
            IOptions<RabbitMqOptions> options)
        {
            _options = options.Value;
        }

        public async Task PublishAsync<T>(
            string routingKey,
            T message,
            CancellationToken cancellationToken = default)
        {
            var factory =
                RabbitMqConnectionFactory.Create(_options);

            await using var connection =
                await factory.CreateConnectionAsync(
                    "CommerceFlow-Publisher",
                    cancellationToken);

            await using var channel =
                await connection.CreateChannelAsync(
                    cancellationToken: cancellationToken);

            await channel.ExchangeDeclareAsync(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            var body =
                JsonSerializer.SerializeToUtf8Bytes(message);

            var properties = new BasicProperties
            {
                ContentType = "application/json",
                Persistent = true,
                MessageId = Guid.NewGuid().ToString(),
                Type = typeof(T).Name
            };

            await channel.BasicPublishAsync(
                exchange: _options.ExchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }
    }
}