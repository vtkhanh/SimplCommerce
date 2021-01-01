using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Module.Catalog.Services;
using SimplCommerce.Module.Catalog.Services.Dtos;
using SimplCommerce.Module.Catalog.ViewModels;

namespace SimplCommerce.Module.Catalog.Controllers
{
    [Authorize(Roles = "admin, vendor")]
    [Route("api/suppliers")]
    public class SupplierApiController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SupplierApiController(ISupplierService supplierService) => _supplierService = supplierService;
        
        public async Task<IActionResult> Get()
        {
            var result = await _supplierService.GetAllAsync();

            return Json(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var model = await _supplierService.GetAsync(id);

            return Json(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Post([FromBody] SupplierFormVm model)
        {
            if (ModelState.IsValid)
            {
                var supplier = new SupplierDto
                {
                    Name = model.Name,
                    Phone = model.Phone,
                    Address = model.Address
                };

                var id = await _supplierService.CreateAsync(supplier);

                return CreatedAtAction(nameof(Get), new { id }, null);
            }

            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Put(long id, [FromBody] SupplierFormVm model)
        {
            if (ModelState.IsValid)
            {
                var dto = new SupplierDto
                {
                    Id = id,
                    Name = model.Name,
                    Phone = model.Phone,
                    Address = model.Address
                };

                await _supplierService.UpdateAsync(dto);

                return Accepted();
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(long id)
        {
            await _supplierService.DeleteAsync(id);
            return NoContent();
        }
    }
}
