﻿using System.ComponentModel.DataAnnotations.Schema;
using SimplCommerce.Infrastructure.Models;
using SimplCommerce.Module.Core.Models;

namespace SimplCommerce.Module.Tax.Models
{
    public class TaxRate : EntityBase
    {
        public string Name { get; set; }

        public long TaxClassId { get; set; }

        public TaxClass TaxClass { get; set; }

        public long CountryId { get; set; }

        public Country Country { get; set; }

        public long? StateOrProvinceId { get; set; }

        public StateOrProvince StateOrProvince { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Rate { get; set; }
    }
}
