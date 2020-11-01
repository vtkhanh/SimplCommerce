using System.Text.Json;

namespace SimplCommerce.Infrastructure.Web.SmartTable
{
    public class Search
    {
        public object PredicateObject { get; set; }

        public TValue ToObject<TValue>() where TValue : new()
        {
            var options = new JsonSerializerOptions { AllowTrailingCommas = true };
            return PredicateObject is object ? JsonSerializer.Deserialize<TValue>(PredicateObject.ToString(), options) : new TValue();
        }
    }
}
