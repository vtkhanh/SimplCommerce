using System.Linq;
using System.Threading.Tasks;
using Moq;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Payments.Models;
using SimplCommerce.Module.Payments.Services;
using SimplCommerce.Test.Shared.MockQueryable;
using Xunit;

namespace SimplCommerce.Module.Payments.Tests
{
    public class PaymentProviderServiceTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Toggle_ShouldWorkAsExpected(bool isEnabled)
        {
            // Arrange
            var mockRepo = new Mock<IRepository<PaymentProvider>>();
            var payment = new PaymentProvider(1);
            var mockPaymentQueryable = new PaymentProvider[] { payment }.AsQueryable().BuildMock();
            mockRepo.Setup(repo => repo.Query()).Returns(mockPaymentQueryable.Object).Verifiable();
            mockRepo.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

            // Action
            var service = new PaymentProviderService(mockRepo.Object);
            var (ok, message) = await service.ToggleAsync(payment.Id, isEnabled);

            // Assert
            mockRepo.Verify();
            Assert.True(ok);
            Assert.Equal(isEnabled, payment.IsEnabled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Toggle_WithOrderNotFound_ShouldFail(bool isEnabled)
        {
            // Arrange
            const int ProviderId = 1;
            var mockRepo = new Mock<IRepository<PaymentProvider>>();
            var mockPaymentQueryable = new PaymentProvider[] { }.AsQueryable().BuildMock();
            mockRepo.Setup(repo => repo.Query()).Returns(mockPaymentQueryable.Object).Verifiable();

            // Action
            var service = new PaymentProviderService(mockRepo.Object);
            var (ok, message) = await service.ToggleAsync(ProviderId, isEnabled);

            // Assert
            mockRepo.Verify();
            Assert.False(ok);
            Assert.Contains(ProviderId.ToString(), message);
        }
    }
}
