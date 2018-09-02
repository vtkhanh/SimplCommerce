using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Core.ViewModels;
using Xunit;

namespace SimplCommerce.Module.Core.Tests
{
    public class UserServiceTest
    {
        public class CreateUserAsync
        {

            private DbContextOptions<SimplDbContext> _options;

            public CreateUserAsync()
            {
                _options = new DbContextOptionsBuilder<SimplDbContext>()
                    .UseInMemoryDatabase(databaseName: nameof(CreateUserAsync))
                    .Options;
            }

            [Fact(Skip="Not completed")]
            public void CreateUserAsync_WithAddress_CanCreateUser()
            {
                // Arrange
                var model = new UserForm
                {
                    Email = "test@mail.com",
                    FullName = "John Doe",
                    PhoneNumber = "1234567890",
                    VendorId = 0
                };

                // Action

                // Assert
            }
        }
    }
}
