using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(
            string routingKey,
            T message,
            CancellationToken cancellationToken = default);
    }
}