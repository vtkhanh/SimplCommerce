using SimplCommerce.Infrastructure;

namespace SimplCommerce.Module.Core.Services.Dtos
{
    public class CustomerDto : UserDto
    {
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Link { get; set; }
        public string Email { get; set; }
        public string Display => $"{FullName}{(PhoneNumber.HasValue() ? $" | {PhoneNumber}" : "")}";
    }
}
