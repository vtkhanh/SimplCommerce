using System.Threading.Tasks;
using System.Collections.Generic;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Catalog.ViewModels;

namespace SimplCommerce.Module.Catalog.Services
{
    public interface ICategoryService
    {
        Task<IList<CategoryListItem>> GetAllAsync();

        Task CreateAsync(Category category);

        Task UpdateAsync(Category category);

        Task DeleteAsync(Category category);
    }
}
