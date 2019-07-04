using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Core.Services.Dtos;
using Xunit;

namespace SimplCommerce.Module.Core.Tests
{
    public class CustomerServiceTest
    {
        public class SearchAsync
        {
            private DbContextOptions<SimplDbContext> _options;
            private IMapper _mapper;

            public SearchAsync()
            {
                _options = new DbContextOptionsBuilder<SimplDbContext>()
                    .UseInMemoryDatabase(databaseName: nameof(SearchAsync))
                    .Options;
                using (var context = new SimplDbContext(_options))
                {
                    var role = new Role { Name = RoleName.Customer };
                    var customerA = new User
                    {
                        IsDeleted = false,
                        FullName = "John Doe",
                        PhoneNumber = "0123456789",
                        Email = "special.john@mail.com",
                        Roles = new HashSet<UserRole> { new UserRole { Role = role }},
                        DefaultShippingAddress = new Address { AddressLine1 = "somewhere" }
                    };
                    var customerB = new User
                    {
                        IsDeleted = true,
                        FullName = "John Henry",
                        PhoneNumber = "987654321",
                        Email = "normal.john@mail.com",
                        Roles = new HashSet<UserRole> { new UserRole { Role = role }},
                        DefaultShippingAddress = new Address { AddressLine1 = "somewhere else" }
                    };
                    var customers = new List<User> { customerA };
                    var userRepo = new Repository<User>(context);
                    userRepo.AddRange(customers);
                    userRepo.SaveChanges();
                }

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<AutoMapperProfile>();
                });
                _mapper = config.CreateMapper();
            }

            [Theory]
            [InlineData("Henry")]
            [InlineData("987")]
            [InlineData("normal")]
            public async Task SearchAsync_ShouldNotReturnDeletedCustomer(string query)
            {
                // Arrange

                // Action
                IList<CustomerDto> searchResult;
                using (var context = new SimplDbContext(_options))
                {
                    var userRepo = new Repository<User>(context);
                    var customerService = new CustomerService(_mapper, null, userRepo);
                    searchResult = (await customerService.SearchAsync(query)).ToList();
                }

                // Assert
                Assert.True(!searchResult.Any());
            }

            [Theory]
            [InlineData("John")]
            [InlineData("012345")]
            [InlineData("special")]
            public async Task SearchAsync_CanSearch_InName_InPhone_InMail(string query)
            {
                // Arrange

                // Action
                IList<CustomerDto> searchResult;
                using (var context = new SimplDbContext(_options))
                {
                    var userRepo = new Repository<User>(context);
                    var customerService = new CustomerService(_mapper, null, userRepo);
                    searchResult = (await customerService.SearchAsync(query)).ToList();
                }

                // Assert
                Assert.True(searchResult.Any());
            }

            [Fact]
            public async Task SearchAsync_ShouldIncludeDefaultShippingAddress()
            {
                // Arrange
                var query = "John";

                // Action
                IList<CustomerDto> searchResult;
                using (var context = new SimplDbContext(_options))
                {
                    var userRepo = new Repository<User>(context);
                    var customerService = new CustomerService(_mapper, null, userRepo);
                    searchResult = (await customerService.SearchAsync(query)).ToList();
                }

                // Assert
                Assert.True(searchResult.Any());
                Assert.Equal("somewhere", searchResult.First().Address);
            }
        }
    }
}
