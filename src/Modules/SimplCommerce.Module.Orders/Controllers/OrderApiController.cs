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
using Microsoft.Extensions.Logging;
using SimplCommerce.Infrastructure.Filters;

namespace SimplCommerce.Module.Orders.Controllers
{
    [Authorize(Roles = "admin, vendor")]
    [Route("api/orders")]
    [ApiController]
    public class OrderApiController : Controller
    {
        private readonly IMediaService _mediaService;
        private readonly IOrderService _orderService;
        private readonly IRepository<Order> _orderRepository;
        private readonly IWorkContext _workContext;

        public OrderApiController(IOrderService orderService, IRepository<Order> orderRepository,
            IMediaService mediaService, IWorkContext workContext)
        {
            _orderService = orderService;
            _orderRepository = orderRepository;
            _mediaService = mediaService;
            _workContext = workContext;
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
            query = query.WhereIf(!User.IsInRole(RoleName.Admin), i => i.VendorId == currentUser.VendorId);

            if (param.Search.PredicateObject != null)
            {
                dynamic search = param.Search.PredicateObject;
                var id = (long?)search.Id;
                var status = (OrderStatus?)search.Status;
                var customerName = (string)search.CustomerName;
                var trackingNumber = (string) search.TrackingNumber;
                var before = (DateTimeOffset?)search.CreatedOn?.before;
                var after = (DateTimeOffset?)search.CreatedOn?.after;
                query = query
                    .Include(i => i.Customer)
                    .WhereIf(id.HasValue, i => i.Id == id.Value)
                    .WhereIf(status.HasValue, i => i.OrderStatus == status.Value)
                    .WhereIf(customerName.HasValue(), i => i.Customer.FullName.Contains(customerName))
                    .WhereIf(trackingNumber.HasValue(), i => i.TrackingNumber.Contains(trackingNumber))
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
                    TrackingNumber = order.TrackingNumber,
                    Total = order.OrderTotal,
                    OrderStatus = order.OrderStatus.ToString(),
                    order.CreatedOn
                });

            return Json(orders);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            var (order, errorMessage) = await _orderService.GetOrderAsync(id);

            return errorMessage.HasValue()
                ? (IActionResult)BadRequest(new { Error = errorMessage }) : Ok(order);
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] OrderFormVm orderForm)
        {
            var (ok, errorMessage) = await _orderService.UpdateOrderAsync(orderForm);
            return ok ? (IActionResult)Accepted() : BadRequest(new { Error = errorMessage });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var order = _orderRepository
                .Query()
                .Include(x => x.ShippingAddress).ThenInclude(x => x.District)
                .Include(x => x.ShippingAddress).ThenInclude(x => x.StateOrProvince)
                .Include(x => x.ShippingAddress).ThenInclude(x => x.Country)
                .Include(x => x.OrderItems).ThenInclude(x => x.Product).ThenInclude(x => x.ThumbnailImage)
                .Include(x => x.OrderItems).ThenInclude(x => x.Product).ThenInclude(x => x.OptionCombinations).ThenInclude(x => x.Option)
                .Include(x => x.Customer)
                .Include(x => x.CreatedBy)
                .FirstOrDefault(x => x.Id == id);

            if (order == null)
            {
                return new NotFoundResult();
            }

            var currentUser = await _workContext.GetCurrentUser();
            if (!User.IsInRole(RoleName.Admin) && order.VendorId != currentUser.VendorId)
            {
                return new BadRequestObjectResult(new { error = "You don't have permission to manage this order" });
            }

            var model = new OrderDetailVm
            {
                Id = order.Id,
                CreatedOn = order.CreatedOn,
                OrderStatus = (int)order.OrderStatus,
                OrderStatusString = order.OrderStatus.ToString(),
                CustomerName = order.Customer.FullName,
                Subtotal = order.SubTotal,
                Discount = order.Discount,
                SubTotalWithDiscount = order.SubTotalWithDiscount,
                // TODO: Add shipping address
                // ShippingAddress = new ShippingAddressVm
                // {
                //     AddressLine1 = order.ShippingAddress.AddressLine1,
                //     AddressLine2 = order.ShippingAddress.AddressLine2,
                //     ContactName = order.ShippingAddress.ContactName,
                //     DistrictName = order.ShippingAddress.District?.Name,
                //     StateOrProvinceName = order.ShippingAddress.StateOrProvince.Name,
                //     Phone = order.ShippingAddress.Phone
                // },
                OrderItems = order.OrderItems.Select(x => new OrderItemVm
                {
                    Id = x.Id,
                    ProductName = x.Product.Name,
                    ProductPrice = x.ProductPrice,
                    ProductImage = _mediaService.GetThumbnailUrl(x.Product.ThumbnailImage),
                    Quantity = x.Quantity,
                    VariationOptions = OrderItemVm.GetVariationOption(x.Product)
                }).ToList()
            };

            return Json(model);
        }

        [HttpPost("change-order-status/{id}")]
        public async Task<IActionResult> ChangeStatus(long id, [FromBody] int statusId)
        {
            var order = _orderRepository.Query().FirstOrDefault(x => x.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            var currentUser = await _workContext.GetCurrentUser();
            if (!User.IsInRole("admin") && order.VendorId != currentUser.VendorId)
            {
                return new BadRequestObjectResult(new { error = "You don't have permission to manage this order" });
            }

            if (Enum.IsDefined(typeof(OrderStatus), statusId))
            {
                order.OrderStatus = (OrderStatus)statusId;
                _orderRepository.SaveChanges();
                return Ok();
            }
            return BadRequest(new { Error = "unsupported order status" });
        }

        [HttpGet("order-status")]
        public IActionResult GetOrderStatus()
        {
            var model = EnumHelper.ToDictionary(typeof(OrderStatus)).Select(x => new { Id = x.Key, Name = x.Value });
            return Json(model);
        }
    }
}
