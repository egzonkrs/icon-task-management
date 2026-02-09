using System.Linq.Expressions;

namespace Icon.SharedKernel.Specifications;

public sealed class OrderExpression<TEntity>
{
    public Expression<Func<TEntity, object>> KeySelector { get; }
    public bool Descending { get; }

    public OrderExpression(Expression<Func<TEntity, object>> keySelector, bool descending)
    {
        KeySelector = keySelector;
        Descending = descending;
    }
}
