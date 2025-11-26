namespace HouseBrokerApplication.Domain.Base;

public interface IRepository<T> where T : IAggregateRoot
{
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<T> GetByIdAsync();
    Task<IEnumerable<T>> GetBySpecification(ISpecification<T> specification);
    IUnitOfWork UnitOfWork { get; }
}
