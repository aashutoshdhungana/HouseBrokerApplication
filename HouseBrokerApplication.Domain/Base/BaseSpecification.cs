using System.Linq.Expressions;

namespace HouseBrokerApplication.Domain.Base
{
    public class BaseSpecification<T>(Expression<Func<T, bool>> criteria) : ISpecification<T> where T : Entity
    {
        public Expression<Func<T, bool>> Criteria => criteria;

        public List<Expression<Func<T, object>>> Includes { get;  } = new();
        public List<string> IncludeStrings { get; } = new();
        public Expression<Func<T, object>>? OrderBy { get; private set; }

        public Expression<Func<T, object>>? OrderByDescending { get; private set; }

        public int Take { get; protected set; } = 10;

        public int Skip { get; protected set; } = 0;

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected void AddInclude(string include)
        {
            IncludeStrings.Add(include);
        }
        protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression;
        }
        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }
    }
}
