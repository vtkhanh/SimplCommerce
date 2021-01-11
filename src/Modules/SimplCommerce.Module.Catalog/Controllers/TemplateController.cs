using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Module.Core.Extensions.Constants;

namespace SimplCommerce.Module.Catalog.Controllers
{
    [Authorize(Policy.CanAccessDashboard)]
    [Route("template/products")]
    [ApiController]
    public class TemplateController : Controller
    {
        private const string ProductListView = "ProductList";
        private const string ProductFormView = "ProductForm";
        private const string ProductListSellerView = "ProductListSeller";
        private const string SupplierListView = "SupplierList";
        private const string SupplierFormView = "SupplierForm";

        [HttpGet("product-list")]
        public IActionResult GetProductList() => User.IsInRole(RoleName.Admin) ? View(ProductListView) : View(ProductListSellerView);

        [HttpGet("product-form")]
        public IActionResult GetProductForm() => View(ProductFormView);

        [HttpGet("supplier-list")]
        public IActionResult GetSupplierList() => View(SupplierListView);

        [HttpGet("supplier-form")]
        public IActionResult GetSupplierForm() => View(SupplierFormView);
    }
}
