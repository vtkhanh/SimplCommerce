using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SimplCommerce.Infrastructure
{
    public static class LinqHelper
    {
        public static async Task<T> FirstIfAsync<T>(this IQueryable<T> source, bool conditional, Expression<Func<T, bool>> predicate) => 
            conditional ? await source.FirstAsync(predicate) : await source.FirstAsync();

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool conditional, Expression<Func<T, bool>> predicate) => 
            conditional ? source.Where(predicate) : source;

        public static IQueryable<T> TakeIf<T>(this IQueryable<T> source, bool conditional, int count) => 
            conditional ? source.Take(count) : source;
    }
}
