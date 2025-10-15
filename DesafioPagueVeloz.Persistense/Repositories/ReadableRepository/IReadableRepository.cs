namespace DesafioPagueVeloz.Persistense.Repositories;

public interface IReadableRepository<T>
{
    Task<T?> GetByIdAsync(Guid id);
}