using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.EntityFrameworkCore;
using Moq;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services;
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

            [Fact()]
            public async Task WithAddress_CanCreateUser()
            {
                // Arrange
                var model = new UserForm
                {
                    Email = "test@mail.com",
                    FullName = "John Doe",
                    PhoneNumber = "1234567890",
                    VendorId = 0,
                    Password = "StrongPassword"
                };
                var mockUserManager = MockHelpers.MockUserManager<User>();
                mockUserManager
                    .Setup(mgr => mgr.CreateAsync(It.IsAny<User>(), model.Password))
                    .ReturnsAsync(IdentityResult.Success)
                    .Verifiable();
                
                // Action
                var userService = new UserService(mockUserManager.Object, null);
                var (result, _) = await userService.CreateUserAsync(model);

                // Assert
                Assert.True(result.Succeeded);
                mockUserManager.VerifyAll();
            }
        }
    }
}
