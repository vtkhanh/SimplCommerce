using System.Collections.Generic;
using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.Services.Constants
{
    internal static class OrderFileConstants
    {
        public static class ColumnIndex
        {
            public const int ExternalOrderId = 0;
            public const int OrderedDate = 1;
            public const int Status = 2;
            public const int TrackingNumber = 4;
            public const int Sku = 12;
            public const int Price = 23;
            public const int Quantity = 24;
            public const int ShippingCost = 35;
            public const int ShippingAmount = 36;
            public const int OrderTotal = 37;
            public const int Username = 44;
            public const int Phone = 46;
            public const int ShippingAddress = 50;
        }

        public static Dictionary<string, OrderStatus> OrderStatusMapping = 
            new Dictionary<string, OrderStatus> {
                { "Hoàn thành", OrderStatus.Complete },
                { "Đã hủy", OrderStatus.Cancelled },
                { "Đang giao", OrderStatus.Shipped },
                { "Chờ giao hàng", OrderStatus.Processing },
                { "Chờ xác nhận", OrderStatus.Processing }
            };

        public const string FileDateTimeFormat = "yyyy-MM-dd HH:mm";
    }
}
