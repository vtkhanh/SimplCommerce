using SimplCommerce.Infrastructure;

namespace SimplCommerce.Module.Catalog.Services.Dtos
{
    public class ProductDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Sku { get; set; }

        public decimal Price { get; set; }

        public decimal Cost { get; set; }

        public int Stock { get; set; }

        public bool HasOptions { get; set; }

        public string ThumbnailImageUrl { get; set; }

        public string Display => $"{(Sku.HasValue() ? $"{Sku} | " : "")}{Name}";
    }
}
