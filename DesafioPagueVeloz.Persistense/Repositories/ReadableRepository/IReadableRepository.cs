using System.Linq.Expressions;

namespace DesafioPagueVeloz.Persistense.Repositories;

public interface IReadableRepository<T>
{
    Task<T?> GetByIdAsync(Guid id);
    Task LoadProperty<TProperty>(T entity, Expression<Func<T, TProperty>> property);
}