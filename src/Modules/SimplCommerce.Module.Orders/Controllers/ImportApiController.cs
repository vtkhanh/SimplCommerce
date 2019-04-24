using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Infrastructure.Helpers;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Orders.ViewModels;

namespace SimplCommerce.Module.Orders.Controllers
{
    [Authorize(Policy = Policy.CanManageOrder)]
    [Route("api/order-import")]
    [ApiController]
    public class ImportApiController : Controller
    {
        private readonly IMediaService _mediaService;

        public ImportApiController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [HttpPost("upload")]
        public async Task<string> UploadOrderFile([FromForm] OrderFileUploadVm model)
        {
            var fileName = model.OrderFile.GetReferenceFileName(Guid.NewGuid());
            await _mediaService.SaveMediaAsync(model.OrderFile.OpenReadStream(), fileName, model.OrderFile.ContentType);
            return fileName;
        }
    }
}
