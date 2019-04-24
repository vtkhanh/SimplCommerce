using Microsoft.AspNetCore.Http;

namespace SimplCommerce.Module.Orders.ViewModels
{
    public class OrderFileUploadVm
    {
        public IFormFile OrderFile { get; set; }
    }
}
