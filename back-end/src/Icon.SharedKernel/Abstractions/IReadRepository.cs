using Icon.SharedKernel.Specifications;

namespace Icon.SharedKernel.Abstractions;

public interface IReadRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
{
    Task<TEntity?> GetByIdAsync(TPrimaryKey id, CancellationToken cancellationToken = default);
    Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
}
