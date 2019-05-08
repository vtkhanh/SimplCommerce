using System.ComponentModel.DataAnnotations.Schema;
using SimplCommerce.Infrastructure.Models;

namespace SimplCommerce.Module.Orders.Models
{
    public class ImportResult : EntityBase
    {
        public long OrderFileId { get; set; }

        public int SuccessCount { get; set; }

        public int FailureCount { get; set; }

        [ForeignKey("OrderFileId")]
        public OrderFile OrderFile { get; set; }
    }
}
