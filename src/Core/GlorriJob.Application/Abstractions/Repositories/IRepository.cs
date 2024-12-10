using System;
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
        Task<IQueryable<T>> GetAll();
        Task<IQueryable<T>> GetAllWhere(Expression<Func<T,bool>> expression, Expression<Func<T, object>> orderBy, bool isTracking, params string[] includes);
        Task<IQueryable<T>> GetFiltered(Expression<Func<T,bool>> expression, Expression<Func<T, object>> orderBy, bool isTracking, params string[] includes);
        Task AddAsync(T entity);
        bool Update(T entity);
        bool Delete(Guid id);
        Task SaveChangesAsync();
    }
}
