using System;

namespace SimplCommerce.Module.Catalog.ViewModels
{
    public class ProductExportVm
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Sku { get; set; }

        public int Weight { get; set; }

        public decimal Price { get; set; }

        public string CreatedOn { get; set; }

        public int Stock { get; set; }
    }
}
