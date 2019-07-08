using System.Collections.Generic;

namespace SimplCommerce.Module.Orders.Services.Dtos
{
    public class ImportResultDto
    {
        IEnumerable<ImportResultDetailDto> Items { get; set; }
    }
}
