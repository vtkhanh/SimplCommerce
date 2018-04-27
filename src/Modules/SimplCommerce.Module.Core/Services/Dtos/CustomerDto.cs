using SimplCommerce.Infrastructure;

namespace SimplCommerce.Module.Core.Services.Dtos
{
    public class CustomerDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Display => $"{FullName}{(PhoneNumber.HasValue() ? $" {PhoneNumber}" : "")}";
    }
}