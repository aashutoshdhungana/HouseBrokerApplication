using HouseBrokerApplication.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace HouseBrokerApplication.Infrastructure.Implementations
{
    public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : Entity, IAggregateRoot
    {
        public IUnitOfWork UnitOfWork => context;

        public void Add(T entity)
        {
            context.Add(entity);
        }

        public void Delete(T entity)
        {
            context.Remove(entity);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await context.FindAsync<T>(id);
        }

        public async Task<IEnumerable<T>> GetBySpecification(ISpecification<T> specification)
        {
            var query = context.Set<T>().AsQueryable();
            var evaluatedQuery = SpecificationEvaluator<T>.GetQuery(query, specification);
            return await evaluatedQuery.ToListAsync();
        }

        public void Update(T entity)
        {
            context.Update(entity);
        }
    }
}
