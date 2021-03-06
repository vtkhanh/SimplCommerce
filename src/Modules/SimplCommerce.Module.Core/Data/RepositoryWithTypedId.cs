﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Models;

namespace SimplCommerce.Module.Core.Data
{
    public class RepositoryWithTypedId<T, TId> : IRepositoryWithTypedId<T, TId> where T : class, IEntityWithTypedId<TId>
    {
        public RepositoryWithTypedId(SimplDbContext context)
        {
            Context = context;
            DbSet = Context.Set<T>();
        }

        protected DbContext Context { get; }

        protected DbSet<T> DbSet { get; }

        public void Add(T entity) => DbSet.Add(entity);

        public void AddRange(IEnumerable<T> entities) => DbSet.AddRange(entities);

        public IDbContextTransaction BeginTransaction() => Context.Database.BeginTransaction();

        public void SaveChanges() => Context.SaveChanges();

        public Task SaveChangesAsync() => Context.SaveChangesAsync();

        public IQueryable<T> Query() => DbSet;

        public IQueryable<T> QueryAsNoTracking() => DbSet.AsNoTracking();

        public void Remove(T entity) => DbSet.Remove(entity);

    }
}
