using Moq;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Services;

namespace SimplCommerce.Module.Orders.Tests.Services.TestableObjects
{
    internal class TestableReportService : ReportService
    {
        public readonly Mock<IRepository<Order>> MockOrderRepo;
        public readonly Mock<IRepository<AppSetting>> MockAppSettingRepo;

        private TestableReportService(Mock<IRepository<Order>> mockOrderRepo, Mock<IRepository<AppSetting>> mockAppSettingRepo) 
            : base(mockOrderRepo.Object, mockAppSettingRepo.Object)
        {
            MockOrderRepo = mockOrderRepo;
            MockAppSettingRepo = mockAppSettingRepo;
        }

        public static TestableReportService Create() =>
            new TestableReportService(new Mock<IRepository<Order>>(), new Mock<IRepository<AppSetting>>());
    }
}
