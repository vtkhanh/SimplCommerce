using System.Threading.Tasks;
using SimplCommerce.Module.Catalog.Models;

namespace SimplCommerce.Module.Catalog.Services
{
    public interface IBrandService
    {
        Task CreateAsync(Brand brand);

        Task UpdateAsync(Brand brand);

        Task DeleteAsync(long id);

        Task DeleteAsync(Brand brand);
    }
}
