using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Catalog.Services;
using Xunit;
using SimplCommerce.Infrastructure.Data;

namespace SimplCommerce.Module.Catalog.Tests
{
    public class ProductServiceTests
    {

        public class AddStockAsync
        {
            private DbContextOptions<SimplDbContext> _options;

            public AddStockAsync()
            {
                _options = new DbContextOptionsBuilder<SimplDbContext>()
                    .UseInMemoryDatabase(databaseName: nameof(AddStockAsync))
                    .Options;

                var product = new Product()
                {
                    Sku = "12345",
                    Stock = 0
                };
                using (var context = new SimplDbContext(_options))
                {
                    var productRepo = new Repository<Product>(context);
                    productRepo.Add(product);
                    productRepo.SaveChanges();
                }
            }

            [Theory]
            [InlineData("12345")]
            public async Task AddStockAsync_WithExistedBarcode_ShouldAddStock(string barcode)
            {
                // Arrange
                var product = await GetProductBySkuAsync(barcode);

                // Act
                bool ok;
                string message;
                using (var context = new SimplDbContext(_options))
                {
                    var productRepo = new Repository<Product>(context);
                    var productService = new ProductService(null, productRepo, null, null, null);
                    (ok, message) = await productService.AddStockAsync(barcode);
                }

                var updatedProduct = await GetProductBySkuAsync(barcode);

                // Assert
                Assert.True(ok);
                Assert.Equal(string.Empty, message);
                Assert.Equal(product.Stock + 1, updatedProduct.Stock);
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("1234567789")]
            public async Task AddStockAsync_WithInvalidBarcode_ShouldReturnFalse(string barcode)
            {
                // Arrange
                Product product = await GetProductBySkuAsync(null);

                // Act
                bool ok;
                string message;
                using (var context = new SimplDbContext(_options))
                {
                    var productRepo = new Repository<Product>(context);
                    var productService = new ProductService(null, productRepo, null, null, null);
                    (ok, message) = await productService.AddStockAsync(barcode);
                }

                Product updatedProduct = await GetProductBySkuAsync(null);

                // Assert
                Assert.False(ok);
                Assert.NotEqual(string.Empty, message);
                Assert.Equal(product.Stock, updatedProduct.Stock);
            }

            private async Task<Product> GetProductBySkuAsync(string sku)
            {
                Product product;
                using (var context = new SimplDbContext(_options))
                {
                    var productRepo = new Repository<Product>(context);
                    product = !string.IsNullOrEmpty(sku)
                        ? await productRepo.Query().FirstAsync(p => p.Sku == sku)
                        : await productRepo.Query().FirstAsync();
                }

                return product;
            }

        }


    }
}
