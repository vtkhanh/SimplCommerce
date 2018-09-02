using AutoMapper;
using Xunit;

namespace SimplCommerce.Module.Catalog.Tests
{
    public class AutoMapperTest
    {
        
        [Fact]
        public void TestConfig()
        {
            Mapper.Initialize(cfg =>
                cfg.AddProfile<SimplCommerce.Module.Catalog.AutoMapperProfile>()
            );

            Mapper.Configuration.AssertConfigurationIsValid();
        }
    }
}
