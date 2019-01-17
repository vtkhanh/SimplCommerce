﻿using Microsoft.AspNetCore.Authorization;
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
        private const string ProductListSellerView = "ProductListSeller";

        [HttpGet("product-list")]
        public IActionResult GetProductList() => User.IsInRole(RoleName.Admin) ? View(ProductListView) : View(ProductListSellerView);
    }
}