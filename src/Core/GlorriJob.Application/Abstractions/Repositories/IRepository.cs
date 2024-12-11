﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GlorriJob.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace GlorriJob.Application.Abstractions.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        DbSet<T> Table { get; }
        Task<T?> GetByIdAsync(Guid id);
        IQueryable<T> GetAll();
        IQueryable<T> GetAll(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderBy, bool isAscending, int skip, int take, bool isTracking, params string[] includes);
        Task<T?> GetFiltered(Expression<Func<T, bool>> expression, bool isTracking, params string[] includes);
        Task<T> AddAsync(T entity);
        T Update(T entity);
        void Delete(T Entity);
        Task SaveChangesAsync();
    }
}
