using SimplCommerce.Infrastructure.Models;

namespace SimplCommerce.Module.Orders.Models
{
    public class ImportResult : EntityBase
    {
        public long ImportFileId { get; set; }

        public int SuccessCount { get; set; }

        public int FailureCount { get; set; }
    }
}
