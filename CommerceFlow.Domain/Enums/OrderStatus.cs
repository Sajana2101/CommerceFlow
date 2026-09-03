using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Domain.Enums
{
    public enum OrderStatus
    {
        PendingPayment = 1,
        Processing = 2,
        Paid = 3,
        Shipped = 4,
        Delivered = 5,
        Cancelled = 6,
        PaymentFailed = 7
    }
}