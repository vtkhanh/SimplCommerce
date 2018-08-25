using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Catalog.Services;
using Xunit;
using SimplCommerce.Infrastructure.Data;
using System.Collections.Generic;
using SimplCommerce.Infrastructure;

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

                using (var context = new SimplDbContext(_options))
                {
                    var products = new List<Product>{
                        new Product() { Sku = "1", Stock = 1 },
                        new Product() { Sku = "12", Stock = 2 },
                        new Product() { Sku = "123", Stock = 3 },
                        new Product() { Sku = "1234", Stock = 4 },
                        new Product() { Sku = "12345", Stock = 5 },
                    };
                    var productRepo = new Repository<Product>(context);
                    productRepo.AddRange(products);
                    productRepo.SaveChanges();
                }
            }

            [Theory]
            [InlineData("12")]
            [InlineData("12345")]
            public async Task AddStockAsync_WithExistedBarcode_ShouldAddStock(string barcode)
            {
                // Arrange
                var product = await GetProductBySkuAsync(barcode);

                // Act
                var (ok, message) = await AddAsync(barcode);
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
                var (ok, message) = await AddAsync(barcode);
                Product updatedProduct = await GetProductBySkuAsync(null);

                // Assert
                Assert.False(ok);
                Assert.Contains(barcode ?? "", message);
                Assert.Equal(product.Stock, updatedProduct.Stock);
            }

            private async Task<(bool, string)> AddAsync(string barcode)
            {
                bool ok;
                string message;
                using (var context = new SimplDbContext(_options))
                {
                    var productRepo = new Repository<Product>(context);
                    var productService = new ProductService(null, productRepo, null, null, null);
                    (ok, message) = await productService.AddStockAsync(barcode);
                }
                return (ok, message);
            }

            private async Task<Product> GetProductBySkuAsync(string sku)
            {
                using (var context = new SimplDbContext(_options))
                {
                    var productRepo = new Repository<Product>(context);
                    var product = await productRepo.Query()
                        .FirstIfAsync(!sku.IsNullOrEmpty(), p => p.Sku == sku);
                    return product;
                }
            }

        }


    }
}
