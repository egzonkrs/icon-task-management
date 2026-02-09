using System.Linq.Expressions;

namespace Icon.SharedKernel.Specifications;

public abstract class Specification<TEntity, TResult> : ISpecification<TEntity, TResult>
{
    private readonly List<Expression<Func<TEntity, object>>> _includes = [];
    private readonly List<string> _includeStrings = [];
    private readonly List<OrderExpression<TEntity>> _orderExpressions = [];

    public Expression<Func<TEntity, bool>>? Criteria { get; protected init; }
    public IReadOnlyList<Expression<Func<TEntity, object>>> Includes => _includes.AsReadOnly();
    public IReadOnlyList<string> IncludeStrings => _includeStrings.AsReadOnly();
    public IReadOnlyList<OrderExpression<TEntity>> OrderExpressions => _orderExpressions.AsReadOnly();
    
    public bool AsNoTracking { get; private set; } = true;
    public Expression<Func<TEntity, TResult>> Selector { get; protected init; } = default!;

    protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
    {
        _includes.Add(includeExpression);
    }

    protected void AddInclude(string includeString)
    {
        _includeStrings.Add(includeString);
    }

    protected void ApplyOrderBy(Expression<Func<TEntity, object>> orderByExpression)
    {
        _orderExpressions.Add(new OrderExpression<TEntity>(orderByExpression, false));
    }

    protected void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByDescendingExpression)
    {
        _orderExpressions.Add(new OrderExpression<TEntity>(orderByDescendingExpression, true));
    }

    protected void AsTracking()
    {
        AsNoTracking = false;
    }
}

public abstract class Specification<TEntity> : Specification<TEntity, TEntity>
{
    protected Specification()
    {
        Selector = entity => entity;
    }
}
