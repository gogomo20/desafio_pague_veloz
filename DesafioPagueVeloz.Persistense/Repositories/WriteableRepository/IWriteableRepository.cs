namespace DesafioPagueVeloz.Persistense.Repositories;

public interface IWriteableRepository<T>
{
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entity);
}