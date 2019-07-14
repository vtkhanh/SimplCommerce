using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SimplCommerce.Infrastructure.Models;

namespace SimplCommerce.Module.Orders.Models
{
    public class ImportResult : EntityBase
    {
        public ImportResult()
        {
            ImportedAt = DateTimeOffset.Now;
        }

        public long OrderFileId { get; set; }

        public int SuccessCount { get; set; }

        public int FailureCount { get; set; }

        public long ImportedById { get; set; }

        public DateTimeOffset ImportedAt { get; set; }

        [ForeignKey("OrderFileId")]
        public OrderFile OrderFile { get; set; }

        public virtual ICollection<ImportResultDetail> ImportResultDetails { get; set; }
    }
}
