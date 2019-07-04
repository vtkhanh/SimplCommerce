using System.ComponentModel.DataAnnotations.Schema;
using SimplCommerce.Infrastructure.Models;
using SimplCommerce.Module.Orders.Models.Enums;

namespace SimplCommerce.Module.Orders.Models
{
    public class ImportResultDetail : EntityBase
    {
        public long ImportResultId { get; set; }

        public string ExternalOrderId { get; set; }

        public ImportResultDetailStatus Status { get; set; }

        public string Message { get; set; }

        [ForeignKey("ImportResultId")]
        public ImportResult ImportResult { get; set; }
    }
}
