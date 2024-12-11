using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.VisualBasic;
using System.Linq.Expressions;

namespace GlorriJob.Persistence.Implementations.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private DbContext _context { get; }

    public Repository(DbContext context)
    {
        _context = context;
    }

    public DbSet<T> Table => _context.Set<T>();

    public async Task<bool> AddAsync(T entity)
    {
        EntityEntry<T> entityEntry =  await Table.AddAsync(entity);
        return entityEntry.State == EntityState.Added;
    }

    public bool Delete(T entity)
    {
        EntityEntry<T> entityEntry = Table.Remove(entity);
        return entityEntry.State == EntityState.Deleted;
    }

    public IQueryable<T> GetAll()
    {
        return Table.AsNoTracking();
    }

    public IQueryable<T> GetAllWhere(Expression<Func<T, bool>> expression, Expression<Func<T, object>>? orderBy = null, bool isTracking = false, params string[] includes)
    {
        IQueryable<T> query = Table.Where(expression);
		if (includes is not null)
		{
			foreach (var include in includes)
			{
				query = query.Include(include);
			}
		}
		if (!isTracking)
        {
            query = query.AsNoTracking();
        }
        if (orderBy is not null)
        {
            query = query.OrderBy(orderBy);
        }
        return query;
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await Table.FindAsync(id);
    }

    public async Task<T?> GetFiltered(Expression<Func<T, bool>> expression, bool isTracking = false, params string[] includes)
    {
        IQueryable<T> query = Table.Where(expression);
        if (includes is not null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
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
        EntityEntry<T> entityEntry = Table.Update(entity);
        return entityEntry.State == EntityState.Modified;
    }
}
