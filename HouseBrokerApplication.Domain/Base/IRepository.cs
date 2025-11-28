namespace HouseBrokerApplication.Domain.Base;

public interface IRepository<T> where T : Entity, IAggregateRoot
{
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetBySpecification(ISpecification<T> specification);
    Task<T?> GetSingleBySpecification(ISpecification<T> specification);
    IUnitOfWork UnitOfWork { get; }
}
