using System.Threading.Tasks;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    public class ImportResultService : IImportResultService
    {
        public async Task<ImportResultDto> GetAsync(long id)
        {
            var result = await Task.FromResult(new ImportResultDto());

            return result;
        }
    }
}
