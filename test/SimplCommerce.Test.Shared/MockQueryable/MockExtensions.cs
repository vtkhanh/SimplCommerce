using System.Collections.Generic;
using System.Linq;
using Moq;
using SimplCommerce.Module.Core.Tests;

namespace SimplCommerce.Test.Shared.MockQueryable
{
    public static class MockExtensions
    {
        public static Mock<IQueryable<TEntity>> BuildMock<TEntity>(this IQueryable<TEntity> data) where TEntity : class
        {
            var mock = new Mock<IQueryable<TEntity>>();
            //mock.As<IAsyncEnumerable<TEntity>>().Setup(d => d.GetAsyncEnumerator(default)).Returns(new TestAsyncEnumerator<TEntity>(data?.GetEnumerator()));
            mock.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<TEntity>(data.Provider));
            mock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(data?.Expression);
            mock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(data?.ElementType);
            mock.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(data?.GetEnumerator());
            return mock;
        }
    }
}
