using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Infrastructure.Helpers;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Orders.Events;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.Orders.Services.Dtos;
using SimplCommerce.Module.Orders.ViewModels;

namespace SimplCommerce.Module.Orders.Controllers
{
    [Authorize(Policy = Policy.CanManageOrder)]
    [Route("api/order-import")]
    [ApiController]
    public class ImportApiController : Controller
    {
        private readonly IWorkContext _workContext;
        private readonly IOrderFileStorageService _fileStorageService;
        private readonly IOrderFileService _orderFileService;
        private readonly IMediator _mediator;

        public ImportApiController(IWorkContext workContext, 
            IOrderFileStorageService fileStorageService, 
            IOrderFileService orderFileService,
            IMediator mediator)
        {
            _workContext = workContext;
            _fileStorageService = fileStorageService;
            _orderFileService = orderFileService;
            _mediator = mediator;
        }

        [HttpPost("upload")]
        public async Task<ActionResult> UploadOrderFile([FromForm] OrderFileUploadVm model)
        {
            var referenceFileName = model.OrderFile.GetReferenceFileName(Guid.NewGuid());
            var currentUser = await _workContext.GetCurrentUser();

            var request = new SaveOrderFileDto
            {
                FileName = model.OrderFile.FileName,
                ReferenceFileName = referenceFileName,
                CreatedOn = DateTimeOffset.Now,
                CreatedById = currentUser.Id
            };
            var orderFileId = await _orderFileService.SaveAsync(request);

            await _fileStorageService.SaveMediaAsync(model.OrderFile.OpenReadStream(), referenceFileName, model.OrderFile.ContentType);

            await _mediator.Send(new ImportOrderRequest(orderFileId, referenceFileName));

            return Accepted();
        }

        [HttpPost("list")]
        public IActionResult List([FromBody] SmartTableParam param)
        {
            var files = _orderFileService.Get(param);

            return Json(files);
        }
    }
}
