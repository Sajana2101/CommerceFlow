using RabbitMQ.Client;

namespace CommerceFlow.Infrastructure.Messaging
{
    internal static class RabbitMqConnectionFactory
    {
        public static ConnectionFactory Create(
            RabbitMqOptions options)
        {
            return new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                AutomaticRecoveryEnabled = true
            };
        }
    }
}