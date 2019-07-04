using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Catalog.Services;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Core.Services.Dtos;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Models.Enums;
using SimplCommerce.Module.Orders.Services.Dtos;
using SimplCommerce.Module.Orders.ViewModels;

namespace SimplCommerce.Module.Orders.Services
{
    internal class OrderImportService : IOrderImportService
    {
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IRepository<Order> _orderRepo;

        public OrderImportService(
            ICustomerService customerService, 
            IOrderService orderService, 
            IProductService productService,
            IRepository<Order> orderRepo)
        {
            _customerService = customerService;
            _orderService = orderService;
            _productService = productService;
            _orderRepo = orderRepo;
        }

        public async Task<bool> ImportAsync(long orderFileId, IEnumerable<ImportingOrderDto> orders)
        {
            var importResult = new ImportResult
            {
                OrderFileId = orderFileId
            };

            foreach (var orderDto in orders)
            {
                try
                {
                    var orderItem = await GetOrderItemAsync(orderDto);

                    if (orderItem is null)
                    {
                        ReportFailure(importResult, orderDto.ExternalOrderId, ImportResultDetailStatus.SkuNotFound, null);
                        continue;
                    }

                    var orderForm = new OrderFormVm
                    {
                        TrackingNumber = orderDto.TrackingNumber,
                        ShippingAmount = orderDto.ShippingAmount,
                        ShippingCost = orderDto.ShippingCost,
                        OrderStatus = orderDto.Status,
                        CustomerId = await GetCustomerIdAsync(orderDto),
                        OrderItems = new List<OrderItemVm> { orderItem }
                    };

                    var feedback = await _orderService.CreateOrderAsync(orderForm);

                    if (feedback.Success)
                    {
                        var importedOrder = await _orderRepo.Query().FirstAsync(order => order.Id == feedback.Result);
                        importedOrder.CompletedOn = orderDto.OrderedDate;
                        importedOrder.ExternalId = orderDto.ExternalOrderId;
                        await _orderRepo.SaveChangesAsync();

                        ReportSuccess(importResult);
                    }
                    else
                    {
                        ReportFailure(importResult, orderDto.ExternalOrderId, ImportResultDetailStatus.Others, feedback.ErrorMessage);
                    }
                }
                catch (Exception exception)
                {
                    ReportFailure(importResult, orderDto.ExternalOrderId, ImportResultDetailStatus.Others, exception.Message);
                }
            }

            return true;
        }

        private void ReportSuccess(ImportResult importResult) => importResult.SuccessCount++;

        private void ReportFailure(ImportResult importResult, string externalOrderId, ImportResultDetailStatus status, string message)
        {
            if (status == ImportResultDetailStatus.Success)
            {
                importResult.SuccessCount++;
            }
            else
            {
                importResult.FailureCount++;
                importResult.ImportResultDetails.Add(new ImportResultDetail
                {
                    ExternalOrderId = externalOrderId,
                    Status = status,
                    Message = message
                });
            }
        }

        private async Task<OrderItemVm> GetOrderItemAsync(ImportingOrderDto orderDto)
        {
            var productId = await _productService.GetProductIdBySkuAsync(orderDto.Sku);

            if (productId <= 0)
                return null;

            return new OrderItemVm
            {
                ProductId = productId,
                Quantity = orderDto.Quantity,
                ProductPrice = orderDto.Price
            };
        }

        private async Task<long> GetCustomerIdAsync(ImportingOrderDto orderDto)
        {
            var customerId = await _customerService.GetCustomerIdByPhoneAsync(orderDto.Phone);

            if (customerId <= 0)
            {
                var dto = new CreateCustomerDto
                {
                    FullName = orderDto.Username,
                    PhoneNumber = orderDto.Phone,
                    Address = orderDto.ShippingAddress,
                    Email = $"{orderDto.Phone}@mail.com",
                    Password = orderDto.Phone
                };
                customerId = await _customerService.CreateCustomerAsync(dto);
            }

            return customerId;
        }
    }
}
