using AutoMapper;
using Xunit;

namespace SimplCommerce.Module.Core.Tests
{
    public class AutoMapperTest
    {
        [Fact]
        public void TestConfig()
        {
            Mapper.Initialize(cfg =>
                cfg.AddProfile<SimplCommerce.Module.Core.AutoMapperProfile>()
            );

            Mapper.Configuration.AssertConfigurationIsValid();
        }
    }
}
