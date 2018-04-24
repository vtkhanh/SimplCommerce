using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SimplCommerce.Infrastructure
{
    public static class LinqHelper
    {
        // public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool conditional, Expression<Func<T, bool>> predicate) => 
            conditional ? source.Where(predicate) : source;
    }
}