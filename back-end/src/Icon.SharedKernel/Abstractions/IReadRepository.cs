using Icon.SharedKernel.Specifications;

namespace Icon.SharedKernel.Abstractions;

public interface IReadRepository<TEntity, in TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
{
    Task<TEntity?> GetByIdAsync(TPrimaryKey id, CancellationToken cancellationToken = default);
    Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default);
    Task<List<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default);
    Task<List<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
}
