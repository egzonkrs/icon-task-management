using Microsoft.EntityFrameworkCore;
using Icon.SharedKernel.Specifications;

namespace Icon.Infrastructure.Data.Specifications;

internal static class SpecificationEvaluator
{
    public static IQueryable<TResult> GetQuery<TEntity, TResult>(IQueryable<TEntity> input, ISpecification<TEntity, TResult> specification) where TEntity : class
    {
        var query = input;

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
        
        if (specification.OrderExpressions.Any())
        {
            IOrderedQueryable<TEntity>? ordered = null;
            foreach (var order in specification.OrderExpressions)
            {
                ordered = ordered is null
                    ? order.Descending ? query.OrderByDescending(order.KeySelector) : query.OrderBy(order.KeySelector)
                    : order.Descending ? ordered.ThenByDescending(order.KeySelector) : ordered.ThenBy(order.KeySelector);
            }

            if (ordered is not null)
            {
                query = ordered;
            }
        }

        if (specification.AsNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query.Select(specification.Selector);
    }
}
