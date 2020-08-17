using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace SimplCommerce.Test.Shared.MockQueryable
{
    public class TestAsyncEnumerable<T> : IAsyncEnumerable<T>, IOrderedQueryable<T>, IAsyncQueryProvider
    {
        private IEnumerable<T> _enumerable;

        public TestAsyncEnumerable(Expression expression) => Expression = expression;

        public TestAsyncEnumerable(IEnumerable<T> enumerable) => _enumerable = enumerable;

        public IAsyncEnumerator<T> GetEnumerator() => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<T>(expression);

        public IQueryable<TEntity> CreateQuery<TEntity>(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);

        public object Execute(Expression expression) => CompileExpressionItem<object>(expression);

        public TResult Execute<TResult>(Expression expression) => CompileExpressionItem<TResult>(expression);

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression) => 
            new TestAsyncEnumerable<TResult>(expression);

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) => 
            Task.FromResult(CompileExpressionItem<TResult>(expression));

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (_enumerable == null)
            {
                _enumerable = CompileExpressionItem<IEnumerable<T>>(Expression);
            }
            return _enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_enumerable == null)
            {
                _enumerable = CompileExpressionItem<IEnumerable<T>>(Expression);
            }
            return _enumerable.GetEnumerator();
        }

        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        public IQueryProvider Provider => this;

        private static TResult CompileExpressionItem<TResult>(Expression expression)
        {
            var rewriter = new TestExpressionVisitor();
            var body = rewriter.Visit(expression);
            var function = Expression.Lambda<Func<TResult>>(body, (IEnumerable<ParameterExpression>)null);
            return function.Compile()();
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
