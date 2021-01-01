using System.ComponentModel.DataAnnotations;
using SimplCommerce.Infrastructure.Models;

namespace SimplCommerce.Module.Catalog.Models
{
    public class Supplier : EntityBase
    {
        public string Name { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(5000)]
        public string Address { get; set; }

        public bool IsDeleted { get; set; }
    }
}
