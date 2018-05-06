using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace SimplCommerce.Infrastructure.Filters
{
    public class AtLeastItemsAttribute : ValidationAttribute
    {
        private readonly int _minItems;
        public AtLeastItemsAttribute(int minElements)
        {
            _minItems = minElements;
        }

        public override bool IsValid(object value)
        {
            var list = value as IList;
            return (list != null && list.Count >= _minItems);
        }
    }
}