using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Linq.Expressions;

namespace GlorriJob.Persistence.Implementations.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly DbContext _context;

    public Repository(DbContext context)
    {
        _context = context;
    }

    public DbSet<T> Table => _context.Set<T>();

    public async Task AddAsync(T entity)
    {
        await Table.AddAsync(entity);
    }

    public bool Delete(Guid id)
    {
        var entity = Table.Find(id);
        if (entity != null)
        {
            Table.Remove(entity);
            return true;
        }
        return false;
    }

    public IQueryable<T> GetAll()
    {
        return Table.AsNoTracking();
    }

    public IQueryable<T> GetAllWhere(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderBy, bool isTracking, params string[] includes)
    {
        IQueryable<T> query = Table.Where(expression);
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        if (!isTracking)
        {
            query = query.AsNoTracking();
        }

        query = query.OrderBy(orderBy);
        return query;
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await Table.FindAsync(id);
    }

    public async Task<T?> GetFiltered(Expression<Func<T, bool>> expression, bool isTracking = false, params string[] includes)
    {
        IQueryable<T> query = Table.Where(expression);
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        if (!isTracking)
        {
            query = query.AsNoTracking();
        }
        return await query.FirstOrDefaultAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public bool Update(T entity)
    {
        Table.Update(entity);
        return true;
    }
}
