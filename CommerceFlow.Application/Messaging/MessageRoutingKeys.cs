using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Messaging
{
    public static class MessageRoutingKeys
    {
        public const string PaymentSucceeded =
            "payment.succeeded";

        public const string PaymentFailed =
            "payment.failed";
    }
}