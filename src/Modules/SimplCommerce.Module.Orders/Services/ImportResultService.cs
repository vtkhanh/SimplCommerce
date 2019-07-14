using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    public class ImportResultService : IImportResultService
    {
        private readonly IRepository<ImportResult> __ImportResultRepo;

        public ImportResultService(IRepository<ImportResult> _importResultRepo)
        {
            __ImportResultRepo = _importResultRepo;
        }

        public async Task<ImportResultDto> GetAsync(long id)
        {
            var resultDto = new ImportResultDto()
            {
                Items = new List<ImportResultDetailDto>()
            };
            var result = await __ImportResultRepo
                .QueryAsNoTracking()
                .Include(item => item.ImportResultDetails)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (result is object)
            {
                resultDto.ImportedAt = result.ImportedAt;
                resultDto.Items = result.ImportResultDetails.Select(item => new ImportResultDetailDto
                {
                    ExternalId = item.ExternalId,
                    Status = item.Status.ToString(),
                    Message = item.Message
                });
            }

            return resultDto;
        }
    }
}
