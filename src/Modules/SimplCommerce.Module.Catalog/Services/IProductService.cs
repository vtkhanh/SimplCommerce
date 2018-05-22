using System.Collections.Generic;
using System.Threading.Tasks;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Catalog.Services.Dtos;

namespace SimplCommerce.Module.Catalog.Services
{
    public interface IProductService
    {
        void Create(Product product);

        void Update(Product product);

        Task Delete(Product product);

        Task<IEnumerable<ProductDto>> Search(string query, int? maxItems = null);

        Task<ProductSettingDto> GetProductSetting();
    }
}
