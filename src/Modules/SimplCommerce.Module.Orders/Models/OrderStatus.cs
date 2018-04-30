using System;

namespace SimplCommerce.Module.Orders.Models
{
    [Flags]
    public enum OrderStatus
    {
        Pending = 0,

        Processing = 1,

        Shipped = 2,

        Paid = 4,

        Complete = 6, // Complete = Shipped and Paid

        Cancelled = 8
    }
}
