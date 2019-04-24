using SimplCommerce.Infrastructure.Models;
using SimplCommerce.Module.Orders.Models.Enums;

namespace SimplCommerce.Module.Orders.Models
{
    public class ImportFile : EntityBase
    {
        public string FileName { get; set; }

        public string UID { get; set; }

        public ImportFileStatus Status { get; set; }

        public int RunCount { get; set; }
    }
}
