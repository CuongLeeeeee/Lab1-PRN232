using Microsoft.EntityFrameworkCore;
using StudentPortal.Repositories.Common;
using StudentPortal.Repositories.Context;
using StudentPortal.Repositories.Interfaces;

namespace StudentPortal.Repositories.Implementations;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);

    public virtual async Task<PagedResult<TEntity>> GetAllAsync(QueryParameters parameters)
    {
        var query = _dbSet.AsNoTracking();
        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<TEntity>
        {
            Items = items,
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        DetachTrackedEntityIfExists(entity);
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is null) return false;
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<bool> ExistsAsync(int id)
        => await _dbSet.FindAsync(id) is not null;

    private void DetachTrackedEntityIfExists(TEntity entity)
    {
        var entityType = _context.Model.FindEntityType(typeof(TEntity));
        var primaryKey = entityType?.FindPrimaryKey();
        if (primaryKey == null) return;

        var keyProperties = primaryKey.Properties;
        // Gather key values from the provided entity
        var entityKeyValues = keyProperties
            .Select(p => p.PropertyInfo?.GetValue(entity))
            .ToArray();

        var trackedEntry = _context.ChangeTracker
            .Entries<TEntity>()
            .FirstOrDefault(e =>
            {
                try
                {
                    var trackedKeyValues = keyProperties
                        .Select(p => p.PropertyInfo?.GetValue(e.Entity))
                        .ToArray();
                    return trackedKeyValues.SequenceEqual(entityKeyValues);
                }
                catch
                {
                    // If something goes wrong comparing keys, don't treat it as a match.
                    return false;
                }
            });

        if (trackedEntry != null)
        {
            trackedEntry.State = EntityState.Detached;
        }
    }
}
