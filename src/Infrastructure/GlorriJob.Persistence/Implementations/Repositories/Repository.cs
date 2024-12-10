using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Linq.Expressions;

namespace GlorriJob.Persistence.Implementations.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    public DbSet<T> Table => throw new NotImplementedException();

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public bool Delete(Guid id)
    {
        var entity = _dbSet.Find(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            return true;
        }
        return false;
    }

    public async Task<IQueryable<T>> GetAll()
    {
        return await Task.FromResult(_dbSet.AsQueryable());
    }

    public async Task<IQueryable<T>> GetAllWhere(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderBy, bool isTracking, params string[] includes)
    {
        return await Task.FromResult(_dbSet.Where(expression));
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IQueryable<T>> GetFiltered(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderBy, bool isTracking, params string[] includes)
    {
        IQueryable<T> query = _dbSet.Where(expression);
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        if (!isTracking)
        {
            query = query.AsNoTracking();
        }
        query = query.OrderBy(orderBy);
        return await Task.FromResult(query);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public bool Update(T entity)
    {
        _dbSet.Update(entity);
        return true;
    }
}
