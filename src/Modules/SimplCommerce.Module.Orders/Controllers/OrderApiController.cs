using System;
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
using SimplCommerce.Infrastructure.ResultTypes;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Core.Services;
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
        private readonly IAppSettingService _appSettingService;

        public OrderApiController(IOrderService orderService,
            IPaymentProviderService paymentProviderService,
            IRepository<Order> orderRepository,
            IWorkContext workContext,
            IAuthorizationService authorizationService,
            ISearchOrderService searchOrderService,
            IAppSettingService appSettingService)
        {
            _orderService = orderService;
            _paymentProviderService = paymentProviderService;
            _orderRepository = orderRepository;
            _workContext = workContext;
            _authorizationService = authorizationService;
            _searchOrderService = searchOrderService;
            _appSettingService = appSettingService;
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
            var feedback = await _orderService.CreateOrderAsync(orderForm);
            return feedback.Success
                ? (IActionResult) Ok(new { Id = feedback.Result })
                : BadRequest(new { Error = feedback.ErrorMessage });
        }

        [HttpPost("list")]
        public async Task<ActionResult> List([FromBody] SmartTableParam param)
        {
            var currentUser = await _workContext.GetCurrentUser();
            var search = param.Search.ToObject<SearchOrderParametersVm>();
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
                    order.ExternalId,
                    Cost = order.OrderTotalCost,
                    Total = order.OrderTotal,
                    StatusId = order.OrderStatus,
                    OrderStatus = order.OrderStatus.ToString(),
                    order.CreatedOn,
                    order.CompletedOn,
                    IsRestricted = !CanEditFullOrder(User.IsInRole(RoleName.Admin), currentUser, order.CreatedById, order.VendorId),
                    CanEdit = CanEditOrder(User.IsInRole(RoleName.Admin), order.OrderStatus, order.CompletedOn)
                });

            return Json(orders);
        }

        [HttpPost("export")]
        public async Task<ActionResult> Export([FromBody] SmartTableParam param)
        {
            var currentUser = await _workContext.GetCurrentUser();
            var search = param.Search.ToObject<SearchOrderParametersVm>();
            search.CanManageOrder = (await _authorizationService.AuthorizeAsync(User, Policy.CanManageOrder)).Succeeded;
            search.UserVendorId = currentUser.VendorId;

            var orders = await _searchOrderService.GetOrdersAsync(search, param.Sort);

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            using (var csvWriter = new CsvWriter(writer))
            {
                csvWriter.WriteRecords(orders);
                writer.Flush();
                var fileName = $"Orders-{DateTime.Now:dd/MM/yyyy}.csv";
                return File(stream.ToArray(), FileContentType.Binary, fileName);
            }
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            var feedback = await _orderService.GetOrderAsync(id);
            return feedback.Success ? Ok(feedback.Result) : (IActionResult)BadRequest(new { Error = feedback.ErrorMessage });
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] OrderFormVm orderForm)
        {
            var getOrderFeedback = await _orderService.GetOrderAsync(orderForm.OrderId);
            if (!getOrderFeedback.Success)
            {
                return BadRequest(new { Error = getOrderFeedback.ErrorMessage });
            }

            var currentUser = await _workContext.GetCurrentUser();
            var order = getOrderFeedback.Result;
            ActionFeedback feedback = ActionFeedback.Succeed();

            if (!CanEditFullOrder(User.IsInRole(RoleName.Admin), currentUser, order.CreatedById, order.VendorId))
            {
                if (order.OrderStatus != orderForm.OrderStatus ||
                    order.TrackingNumber != orderForm.TrackingNumber ||
                    order.PaymentProviderId != orderForm.PaymentProviderId)
                {
                    feedback = await _orderService.UpdateOrderStateAsync(orderForm);
                }
            }
            else
            {
                feedback = await _orderService.UpdateOrderAsync(orderForm);
            }

            return feedback.Success ? (IActionResult)Accepted() : BadRequest(new { Error = feedback.ErrorMessage });
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
            var model = EnumHelper.ToDictionary(typeof(OrderStatus)).Select(x => new { Id = Convert.ToInt32(x.Key), Name = x.Value });
            return Json(model);
        }

        [HttpPut("change-tracking-number")]
        public async Task<IActionResult> ChangeTrackingNumber(OrderUpdateVm order)
        {
            var feedback = await _orderService.UpdateTrackingNumberAsync(order.OrderId, order.TrackingNumber);
            return feedback.Success ? Ok() : (IActionResult)BadRequest(new { Error = feedback.ErrorMessage });
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

        [HttpGet("shopee-fee")]
        public async Task<IActionResult> GetShopeeFee()
        {
            var shopeeFeeSetting = await _appSettingService.GetAsync(AppSettingKey.ShopeeFee);
            int.TryParse(shopeeFeeSetting.Value, out int shopeeFee);
            return Json(shopeeFee);
        }

        [HttpPut("update-multiple-statuses")]
        public async Task<IActionResult> UpdateMultipleStatuses(UpdateMultipleStatusesVm request)
        {
            var (result, error) = await _orderService.UpdateStatusesAsync(request.OrderIds, request.Status);
            return result != null ? Ok(result) : (IActionResult)BadRequest(new { Error = error });
        }

        private static bool CanEditFullOrder(bool isAdmin, Core.Models.User currentUser, long createdById, long? vendorId) =>
            isAdmin || createdById == currentUser.Id || (vendorId.HasValue && vendorId == currentUser.VendorId);

        private static bool CanEditOrder(bool isAdmin, OrderStatus orderStatus, DateTimeOffset? completedOn) =>
            isAdmin || orderStatus != OrderStatus.Complete || completedOn > DateTimeOffset.Now.AddDays(-1);
    }
}
