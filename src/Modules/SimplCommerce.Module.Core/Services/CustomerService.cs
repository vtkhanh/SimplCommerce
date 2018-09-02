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

namespace SimplCommerce.Module.Core.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<User> _userRepo;

        public CustomerService(IMapper mapper, IRepository<User> userRepo)
        {
            _mapper = mapper;
            _userRepo = userRepo;
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
