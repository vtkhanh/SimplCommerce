using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Services.Dtos;
using SimplCommerce.Module.Orders.ViewModels;

namespace SimplCommerce.Module.Orders.Services
{
    internal class OrderImportService : IOrderImportService
    {
        private readonly IOrderService _orderService;
        private readonly IRepository<Order> _orderRepo;

        public OrderImportService(IOrderService orderService, IRepository<Order> orderRepo)
        {
            _orderService = orderService;
            _orderRepo = orderRepo;
        }

        public async Task<bool> ImportAsync(IEnumerable<ImportingOrderDto> orders)
        {
            // 1. Get Customer
            // 2. Get Product
            // 3. Create Order
            // 4. Update imported orders with ExternalOrderId & OrderedDate
            foreach (var orderDto in orders)
            {
                var orderForm = new OrderFormVm
                {
                    TrackingNumber = orderDto.TrackingNumber,
                    ShippingAmount = orderDto.ShippingAmount,
                    ShippingCost = orderDto.ShippingCost,
                    OrderStatus = orderDto.Status,
                    CustomerId = GetCustomerId(orderDto),
                    OrderItems = new List<OrderItemVm> { GetOrderItem(orderDto) }
                };

                var feedback = await _orderService.CreateOrderAsync(orderForm);

                if (feedback.Success)
                {
                    var importedOrder = await _orderRepo.Query().FirstAsync(order => order.Id == feedback.Result);
                    importedOrder.CompletedOn = orderDto.OrderedDate;
                    importedOrder.ExternalId = orderDto.ExternalOrderId;
                    await _orderRepo.SaveChangesAsync();
                }
            }

            return true;
        }

        private OrderItemVm GetOrderItem(ImportingOrderDto orderDto)
        {
            throw new NotImplementedException();
        }

        private long GetCustomerId(ImportingOrderDto orderDto)
        {
            throw new NotImplementedException();
        }
    }
}
