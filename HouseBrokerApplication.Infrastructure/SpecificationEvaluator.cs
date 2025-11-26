using HouseBrokerApplication.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace HouseBrokerApplication.Infrastructure
{
    public static class SpecificationEvaluator<T> where T : Entity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
        {
            var query = inputQuery;
            // Apply criteria
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }
            // Apply includes
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
            // Apply ordering
            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }
            // Apply paging
            query = query.Skip(spec.Skip).Take(spec.Take);
            return query;
        }
    }
}
