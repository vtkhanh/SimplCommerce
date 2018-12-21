using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.ViewModels;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Orders.Services;

namespace SimplCommerce.Module.Orders.Controllers
{
    [Authorize(Roles = "admin, vendor, seller")]
    [Route("api/orders")]
    [ApiController]
    public class OrderApiController : Controller
    {
        private readonly IMediaService _mediaService;
        private readonly IOrderService _orderService;
        private readonly IRepository<Order> _orderRepository;
        private readonly IWorkContext _workContext;
        private readonly IAuthorizationService _authorizationService;

        public OrderApiController(IOrderService orderService, IRepository<Order> orderRepository,
            IMediaService mediaService, IWorkContext workContext, IAuthorizationService authorizationService)
        {
            _orderService = orderService;
            _orderRepository = orderRepository;
            _mediaService = mediaService;
            _workContext = workContext;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<ActionResult> Get(int status, int numRecords)
        {
            var orderStatus = (OrderStatus)status;
            if ((numRecords <= 0) || (numRecords > 100))
            {
                numRecords = 5;
            }

            var query = _orderRepository
                .Query()
                .Where(x => x.OrderStatus == orderStatus);

            var currentUser = await _workContext.GetCurrentUser();
            if (!User.IsInRole("admin"))
            {
                query = query.Where(x => x.VendorId == currentUser.VendorId);
            }

            var model = query.OrderByDescending(x => x.CreatedOn)
                .Take(numRecords)
                .Select(x => new
                {
                    x.Id,
                    CustomerName = x.CreatedBy.FullName,
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
            IQueryable<Order> query = _orderRepository.Query();

            var currentUser = await _workContext.GetCurrentUser();
            var canManageOrder = (await _authorizationService.AuthorizeAsync(User, Policy.CanManageOrder)).Succeeded;
            query = query.WhereIf(!canManageOrder, i => i.VendorId == currentUser.VendorId);

            if (param.Search.PredicateObject != null)
            {
                dynamic search = param.Search.PredicateObject;
                var id = (long?)search.Id;
                var status = (OrderStatus?)search.Status;
                var customerName = (string)search.CustomerName;
                var trackingNumber = (string)search.TrackingNumber;
                var createdBy = (string)search.CreatedBy;
                var before = (DateTimeOffset?)search.CreatedOn?.before;
                var after = (DateTimeOffset?)search.CreatedOn?.after;
                query = query
                    .Include(i => i.Customer)
                    .Include(i => i.CreatedBy)
                    .WhereIf(id.HasValue, i => i.Id == id.Value)
                    .WhereIf(status.HasValue, i => i.OrderStatus == status.Value)
                    .WhereIf(customerName.HasValue(), i => i.Customer.FullName.Contains(customerName))
                    .WhereIf(trackingNumber.HasValue(), i => i.TrackingNumber.Contains(trackingNumber))
                    .WhereIf(createdBy.HasValue(), i => i.CreatedBy.FullName.Contains(createdBy))
                    .WhereIf(before.HasValue, x => x.CreatedOn <= before)
                    .WhereIf(after.HasValue, x => x.CreatedOn >= after)
                    ;
            }

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
                    CanEdit = CanEditFullOrder(currentUser, order.CreatedById, order.VendorId)
                });

            return Json(orders);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            var (order, errorMessage) = await _orderService.GetOrderAsync(id);
            var currentUser = await _workContext.GetCurrentUser();
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

            if (!CanEditFullOrder(currentUser, order.CreatedById, order.VendorId))
            {
                if (order.OrderStatus != orderForm.OrderStatus)
                {
                    (_, errorMessage) = await _orderService.UpdateStatusAsync(orderForm.OrderId, orderForm.OrderStatus);
                }
                if (!errorMessage.HasValue() && order.TrackingNumber != orderForm.TrackingNumber)
                {
                    (_, errorMessage) = await _orderService.UpdateTrackingNumberAsync(orderForm.OrderId, orderForm.TrackingNumber);
                }
            }
            else
            {
                (_, errorMessage) = await _orderService.UpdateOrderAsync(orderForm);
            }

            return !errorMessage.HasValue() ? (IActionResult)Accepted() : BadRequest(new { Error = errorMessage });
        }

        [HttpPut("change-order-status")]
        public async Task<IActionResult> ChangeStatus(OrderUpdateVm order)
        {
            var (result, error) = await _orderService.UpdateStatusAsync(order.OrderId, order.Status);
            return result != null ? Ok(result) : (IActionResult) BadRequest(new { Error = error });
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
            return ok ? Ok() : (IActionResult) BadRequest(new { Error = error });
        }

        private bool CanEditFullOrder(Core.Models.User currentUser, long createdById, long? vendorId) => 
            User.IsInRole(RoleName.Admin) || createdById == currentUser.Id || (vendorId.HasValue && vendorId == currentUser.VendorId);

    }
}
