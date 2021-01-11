using System;
using SimplCommerce.Infrastructure.Models;

namespace SimplCommerce.Module.Catalog.Models
{
    public class StockImport : EntityBase
    {
        public long ProductId { get; set; }
        public DateTimeOffset Date { get; set; }
        public int Quantity { get; set; }
        public long SupplierId { get; set; }
        public decimal Cost { get; set; }
        public decimal NewPrice { get; set; }
        public Supplier Supplier { get; set; }
        public Product Product { get; set; }
    }
}
