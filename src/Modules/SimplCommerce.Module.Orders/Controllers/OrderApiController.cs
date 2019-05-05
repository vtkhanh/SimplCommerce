﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.Orders.ViewModels;
using SimplCommerce.Module.Payments.Services;

namespace SimplCommerce.Module.Orders.Controllers
{
    [Authorize(Policy = Policy.CanAccessDashboard)]
    [Route("api/orders")]
    [ApiController]
    public class OrderApiController : Controller
    {
        private const int DashboardRecordNumber = 10;

        private readonly IOrderService _orderService;
        private readonly IPaymentProviderService _paymentProviderService;
        private readonly IRepository<Order> _orderRepository;
        private readonly IWorkContext _workContext;
        private readonly IAuthorizationService _authorizationService;
        private readonly ISearchOrderService _searchOrderService;

        public OrderApiController(IOrderService orderService,
            IPaymentProviderService paymentProviderService,
            IRepository<Order> orderRepository,
            IWorkContext workContext,
            IAuthorizationService authorizationService,
            ISearchOrderService searchOrderService)
        {
            _orderService = orderService;
            _paymentProviderService = paymentProviderService;
            _orderRepository = orderRepository;
            _workContext = workContext;
            _authorizationService = authorizationService;
            _searchOrderService = searchOrderService;
        }

        [HttpGet]
        public IActionResult Get(int status, int numRecords)
        {
            var orderStatus = (OrderStatus)status;
            if ((numRecords <= 0) || (numRecords > 100))
            {
                numRecords = DashboardRecordNumber;
            }

            var query = _orderRepository.QueryAsNoTracking()
                .Include(item => item.Customer)
                .Include(item => item.CreatedBy)
                .Where(x => x.OrderStatus == orderStatus);

            var model = query.OrderByDescending(x => x.CreatedOn)
                .Take(numRecords)
                .Select(x => new
                {
                    x.Id,
                    CustomerName = x.Customer.FullName,
                    CreatedBy = x.CreatedBy.FullName,
                    x.SubTotal,
                    OrderStatus = x.OrderStatus.ToString(),
                    x.CreatedOn
                });

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderFormVm orderForm)
        {
            var (orderId, errorMessage) = await _orderService.CreateOrderAsync(orderForm);
            return orderId > 0
                ? (IActionResult)Ok(new { Id = orderId })
                : BadRequest(new { Error = errorMessage });
        }

        [HttpPost("list")]
        public async Task<ActionResult> List([FromBody] SmartTableParam param)
        {
            var currentUser = await _workContext.GetCurrentUser();
            var search = param.Search.PredicateObject?.ToObject<SearchOrderParametersVm>() ?? new SearchOrderParametersVm();
            search.CanManageOrder = (await _authorizationService.AuthorizeAsync(User, Policy.CanManageOrder)).Succeeded;
            search.UserVendorId = currentUser.VendorId;

            var query = _searchOrderService.BuildQuery(search);

            var orders = query.ToSmartTableResult(
                param,
                order => new
                {
                    order.Id,
                    CustomerName = order.Customer.FullName,
                    CreatedBy = order.CreatedBy.FullName,
                    order.TrackingNumber,
                    Cost = order.OrderTotalCost,
                    Total = order.OrderTotal,
                    StatusId = order.OrderStatus,
                    OrderStatus = order.OrderStatus.ToString(),
                    order.CreatedOn,
                    order.CompletedOn,
                    IsRestricted = !CanEditFullOrder(currentUser, order.CreatedById, order.VendorId),
                    CanEdit = CanEditOrder(order)
                });

            return Json(orders);
        }

        
        [HttpPost("export")]
        public async Task<ActionResult> Export([FromBody] SmartTableParam param)
        {
            var currentUser = await _workContext.GetCurrentUser();
            var search = param.Search.PredicateObject?.ToObject<SearchOrderParametersVm>() ?? new SearchOrderParametersVm();
            search.CanManageOrder = (await _authorizationService.AuthorizeAsync(User, Policy.CanManageOrder)).Succeeded;
            search.UserVendorId = currentUser.VendorId;

            var orders = await _searchOrderService.GetOrdersAsync(search, param.Sort);

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            using (var csvWriter = new CsvWriter(writer))
            {
                csvWriter.WriteRecords(orders);
                writer.Flush();
                var fileName = $"Orders-{DateTime.Now.ToString("dd/MM/yyyy")}.csv";
                return File(stream.ToArray(), FileContentType.Binary, fileName);
            }
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            var (order, errorMessage) = await _orderService.GetOrderAsync(id);
            return errorMessage.HasValue() ? (IActionResult)BadRequest(new { Error = errorMessage }) : Ok(order);
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] OrderFormVm orderForm)
        {
            var (order, errorMessage) = await _orderService.GetOrderAsync(orderForm.OrderId);
            if (order == null)
            {
                return BadRequest(new { Error = errorMessage });
            }

            var currentUser = await _workContext.GetCurrentUser();

            bool ok = true;
            if (!CanEditFullOrder(currentUser, order.CreatedById, order.VendorId))
            {
                if (order.OrderStatus != orderForm.OrderStatus ||
                    order.TrackingNumber != orderForm.TrackingNumber ||
                    order.PaymentProviderId != orderForm.PaymentProviderId)
                {
                    (ok, errorMessage) = await _orderService.UpdateOrderStateAsync(orderForm);
                }
            }
            else
            {
                (ok, errorMessage) = await _orderService.UpdateOrderAsync(orderForm);
            }

            return ok ? (IActionResult)Accepted() : BadRequest(new { Error = errorMessage });
        }

        [HttpPut("change-order-status")]
        public async Task<IActionResult> ChangeStatus(OrderUpdateVm order)
        {
            var (result, error) = await _orderService.UpdateStatusAsync(order.OrderId, order.Status);
            return result != null ? Ok(result) : (IActionResult)BadRequest(new { Error = error });
        }

        [HttpGet("order-status")]
        public IActionResult GetOrderStatus()
        {
            var model = EnumHelper.ToDictionary(typeof(OrderStatus)).Select(x => new { Id = x.Key, Name = x.Value });
            return Json(model);
        }

        [HttpPut("change-tracking-number")]
        public async Task<IActionResult> ChangeTrackingNumber(OrderUpdateVm order)
        {
            var (ok, error) = await _orderService.UpdateTrackingNumberAsync(order.OrderId, order.TrackingNumber);
            return ok ? Ok() : (IActionResult)BadRequest(new { Error = error });
        }

        [HttpGet("status-list")]
        public IActionResult GetStatusList()
        {
            var selectList = Enum.GetValues(typeof(OrderStatus))
               .Cast<OrderStatus>()
               .Select(t => new SelectListItem
               {
                   Value = ((int)t).ToString(),
                   Text = t.ToString()
               }).ToList();

            return Json(selectList);
        }

        [HttpGet("payment-list")]
        public async Task<IActionResult> GetPaymentList()
        {
            var payments = await _paymentProviderService.GetListAsync(true);
            var selectList = payments.Select(item => new SelectListItem
            {
                Value = item.Id.ToString(),
                Text = item.Description
            });
            return Json(selectList);
        }

        [HttpPut("update-multiple-statuses")]
        public async Task<IActionResult> UpdateMultipleStatuses(UpdateMultipleStatusesVm request)
        {
            var (result, error) = await _orderService.UpdateStatusesAsync(request.OrderIds, request.Status);
            return result != null ? Ok(result) : (IActionResult)BadRequest(new { Error = error });
        }

        private bool CanEditFullOrder(Core.Models.User currentUser, long createdById, long? vendorId) =>
            User.IsInRole(RoleName.Admin) || createdById == currentUser.Id || (vendorId.HasValue && vendorId == currentUser.VendorId);

        private bool CanEditOrder(Order order) =>
            User.IsInRole(RoleName.Admin) || order.OrderStatus != OrderStatus.Complete || order.CompletedOn > DateTimeOffset.Now.AddDays(-1);
    }
}
