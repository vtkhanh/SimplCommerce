using System.Collections.Generic;
using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.Services.Constants
{
    internal static class OrderFileConstants
    {
        public static class ColumnIndex
        {
            public const int ExternalId = 0;
            public const int OrderedDate = 1;
            public const int Status = 2;
            public const int TrackingNumber = 3;
            public const int Sku = 4;
            public const int Price = 5;
            public const int Quantity = 6;
            public const int ShippingCost = 7;
            public const int ShippingAmount = 8;
            public const int OrderTotal = 9;
            public const int Username = 10;
            public const int Phone = 11;
            public const int ShippingAddress = 12;
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
