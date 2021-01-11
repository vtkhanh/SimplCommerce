using System;

namespace SimplCommerce.Module.Catalog.Services.Dtos
{
    public class StockImportDto
    {
        public long Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public long SupplierId { get; set; }
        public string SupplierName { get; set; }
        public decimal Cost { get; set; }
        public decimal NewPrice { get; set; }
    }
}
