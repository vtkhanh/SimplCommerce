using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SimplCommerce.Infrastructure;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.ShoppingCart.ViewModels;

namespace SimplCommerce.Module.Orders.ViewModels
{
    public class OrderItemVm
    {
        public long Id { get; set; }

        public long ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSku { get; set; }

        public string ProductImage { get; set; }

        public decimal ProductPrice { get; set; }

        public int Stock { get; set; }

        public string ProductPriceString => ProductPrice.ToString("C");

        public int Quantity { get; set; }

        public decimal Total => Quantity * ProductPrice;

        // TODO: Use either Total or Subtotal
        public decimal SubTotal => Quantity * ProductPrice;

        public string TotalString => SubTotal.ToString("C");

        public string Display => $"{ProductName}{(ProductSku.HasValue() ? $" {ProductSku}" : "")}";

        public IEnumerable<ProductVariationOptionVm> VariationOptions { get; set; } =
            new List<ProductVariationOptionVm>();

        public static IEnumerable<ProductVariationOptionVm> GetVariationOption(Product variation)
        {
            if (variation == null)
            {
                return new List<ProductVariationOptionVm>();
            }

            return variation.OptionCombinations.Select(x => new ProductVariationOptionVm
            {
                OptionName = x.Option.Name,
                Value = x.Value
            });
        }
    }
}
