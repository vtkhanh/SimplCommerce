using System.ComponentModel.DataAnnotations;

namespace SimplCommerce.Module.Catalog.ViewModels
{
    public class StockImportVm
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Product must be created first")]
        public long ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Supplier must be selected")]
        public long SupplierId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be over 0")]
        public int Quantity { get; set; }

        [Required]
        public decimal Cost { get; set; }

        [Required]
        public decimal NewPrice { get; set; }
    }
}
