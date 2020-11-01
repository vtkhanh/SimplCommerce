using System;
using System.Collections.Generic;
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

        public async Task<ImportResult> ImportAsync(IEnumerable<ImportingOrderDto> orders)
        {
            var importResult = new ImportResult
            {
                ImportResultDetails = new List<ImportResultDetail>()
            };

            foreach (var orderDto in orders)
            {
                try
                {
                    if (await ValidateOrderIsImportedAsync(orderDto.ExternalId))
                    {
                        Report(importResult, orderDto.ExternalId, ImportResultDetailStatus.Imported, null);
                        continue;
                    }

                    var orderItem = await GetOrderItemAsync(orderDto);
                    if (orderItem is null)
                    {
                        Report(importResult, orderDto.ExternalId, ImportResultDetailStatus.SkuNotFound, null);
                        continue;
                    }

                    var orderForm = new OrderFormVm
                    {
                        ExternalId = orderDto.ExternalId,
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
                        Report(importResult, orderDto.ExternalId, ImportResultDetailStatus.Success, null);
                    }
                    else
                    {
                        Report(importResult, orderDto.ExternalId, ImportResultDetailStatus.Others, feedback.ErrorMessage);
                    }
                }
                catch (Exception exception)
                {
                    // TODO: log exception to AppInsights

                    Report(importResult, orderDto.ExternalId, ImportResultDetailStatus.Others, exception.Message);
                }
            }

            return importResult;
        }

        private async Task<bool> ValidateOrderIsImportedAsync(string externalId)
        {
            var importedOrder = await _orderRepo
                .QueryAsNoTracking()
                .FirstOrDefaultAsync(order => !string.IsNullOrEmpty(order.ExternalId) && order.ExternalId == externalId);

            return importedOrder is object;
        }

        private void Report(ImportResult importResult, string externalId, ImportResultDetailStatus status, string message)
        {
            if (status == ImportResultDetailStatus.Success)
            {
                importResult.SuccessCount++;
            }
            else
            {
                importResult.FailureCount++;
            }

            importResult.ImportResultDetails.Add(new ImportResultDetail
            {
                ExternalId = externalId,
                Status = status,
                Message = message
            });
        }

        private async Task<OrderItemVm> GetOrderItemAsync(ImportingOrderDto orderDto)
        {
            var productId = await _productService.GetProductIdBySkuAsync(orderDto.Sku);

            if (productId <= 0)
            {
                return null;
            }

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
