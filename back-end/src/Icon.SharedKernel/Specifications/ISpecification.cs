using System.Linq.Expressions;

namespace Icon.SharedKernel.Specifications;

/// <summary>
/// Describes a query against an entity — filters, sorting, includes, and projection.
/// </summary>
public interface ISpecification<TEntity, TResult>
{
    /// <summary>
    /// Filter expression (WHERE clause). Null means no filter.
    /// </summary>
    Expression<Func<TEntity, bool>>? Criteria { get; }

    /// <summary>
    /// Navigation properties to eager-load.
    /// </summary>
    IReadOnlyList<Expression<Func<TEntity, object>>> Includes { get; }

    /// <summary>
    /// String-based include paths for nested navigation properties.
    /// </summary>
    IReadOnlyList<string> IncludeStrings { get; }

    /// <summary>
    /// Ordering rules to apply to the result set.
    /// </summary>
    IReadOnlyList<OrderExpression<TEntity>> OrderExpressions { get; }

    /// <summary>
    /// When true, the query does not track returned entities for changes.
    /// </summary>
    bool AsNoTracking { get; }

    /// <summary>
    /// Projection expression that maps each entity to the result shape.
    /// </summary>
    Expression<Func<TEntity, TResult>> Selector { get; }
}

/// <summary>
/// A specification that returns the entity itself (no projection).
/// </summary>
public interface ISpecification<TEntity> : ISpecification<TEntity, TEntity>;
