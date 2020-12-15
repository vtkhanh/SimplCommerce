using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SimplCommerce.Infrastructure.Models;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Orders.Models.Enums;

namespace SimplCommerce.Module.Orders.Models
{
    public class OrderFile : EntityBase
    {
        public string FileName { get; set; }

        public string ReferenceFileName { get; set; }

        public ImportFileStatus Status { get; set; }

        public long CreatedById { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("CreatedById")]
        public User CreatedBy { get; set; }

        public virtual ICollection<ImportResult> ImportResults { get; set; }
    }
}
