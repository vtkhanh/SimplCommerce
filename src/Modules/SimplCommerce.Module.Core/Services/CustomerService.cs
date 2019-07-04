using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services.Dtos;
using SimplCommerce.Module.Core.ViewModels;

namespace SimplCommerce.Module.Core.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IRepository<User> _userRepo;

        public CustomerService(IMapper mapper, IUserService userService, IRepository<User> userRepo)
        {
            _mapper = mapper;
            _userService = userService;
            _userRepo = userRepo;
        }

        public async Task<long> CreateCustomerAsync(CreateCustomerDto cusomter)
        {
            var model = new UserForm
            {
                FullName = cusomter.FullName,
                PhoneNumber = cusomter.PhoneNumber,
                Address = cusomter.Address,
                Link = cusomter.Link,
                Email = cusomter.Email,
                RoleIds = new List<long> { (long) RoleId.Customer },
                Password = cusomter.Password
            };

            var (_, userId) = await _userService.CreateUserAsync(model);

            return userId;
        }

        public async Task<long> GetCustomerIdByPhoneAsync(string phone)
        {
            var customer = await _userRepo.QueryAsNoTracking().SingleOrDefaultAsync(user => user.PhoneNumber == phone);
            return customer?.Id ?? 0;
        }

        public async Task<IEnumerable<CustomerDto>> SearchAsync(string query)
        {
            var customers = await _userRepo.Query()
                .Include(i => i.Roles)
                .Include(i => i.DefaultShippingAddress)
                .Where(i => !i.IsDeleted && i.Roles.Any(r => r.Role.Name == RoleName.Customer))
                .WhereIf(query.HasValue(), i => i.FullName.Contains(query) || i.PhoneNumber.Contains(query) || i.Email.Contains(query))
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }
    }
}
