using System.Linq.Expressions;

namespace Icon.SharedKernel.Specifications;

public interface ISpecification<TEntity, TResult>
{
    Expression<Func<TEntity, bool>>? Criteria { get; }
    IReadOnlyList<Expression<Func<TEntity, object>>> Includes { get; }
    IReadOnlyList<string> IncludeStrings { get; }
    IReadOnlyList<OrderExpression<TEntity>> OrderExpressions { get; }
    bool AsNoTracking { get; }
    Expression<Func<TEntity, TResult>> Selector { get; }
}

public interface ISpecification<TEntity> : ISpecification<TEntity, TEntity>;
