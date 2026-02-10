using Microsoft.EntityFrameworkCore;
using Icon.SharedKernel.Abstractions;
using Icon.SharedKernel.Specifications;
using Icon.Infrastructure.Data.Specifications;

namespace Icon.Infrastructure.Data.Repositories;

/// <summary>
/// Generic EF Core repository implementation with specification support.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TPrimaryKey">The entity key type.</typeparam>
public class EfRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
    where TEntity : class, IEntity<TPrimaryKey>
{
    private readonly DbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    public EfRepository(DbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator.GetQuery(_dbSet.AsQueryable(), specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator.GetQuery(_dbSet.AsQueryable(), specification).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator.GetQuery(_dbSet.AsQueryable(), specification).ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await CountInternalAsync(specification, cancellationToken);
    }

    public async Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(TEntity entity) => _dbSet.Update(entity);
    public void UpdateRange(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);
    public void Remove(TEntity entity) => _dbSet.Remove(entity);
    public void RemoveRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

    private async Task<int> CountInternalAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken)
    {
        var query = _dbSet.AsQueryable();
        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        foreach (var includeExpression in specification.Includes)
        {
            query = query.Include(includeExpression);
        }

        foreach (var includeString in specification.IncludeStrings)
        {
            query = query.Include(includeString);
        }

        if (specification.AsNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.CountAsync(cancellationToken);
    }
}
