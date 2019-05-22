using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Infrastructure.Helpers;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Core.Services;
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

        public ImportApiController(IWorkContext workContext, IOrderFileStorageService fileStorageService, IOrderFileService orderFileService)
        {
            _workContext = workContext;
            _fileStorageService = fileStorageService;
            _orderFileService = orderFileService;
        }

        [HttpPost("upload")]
        public async Task<string> UploadOrderFile([FromForm] OrderFileUploadVm model)
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
            await _orderFileService.SaveAsync(request);

            await _fileStorageService.SaveMediaAsync(model.OrderFile.OpenReadStream(), referenceFileName, model.OrderFile.ContentType);

            return referenceFileName;
        }

        [HttpPost("list")]
        public IActionResult List([FromBody] SmartTableParam param)
        {
            var files = _orderFileService.GetOrderFiles(param);

            return Json(files);
        }
    }
}
