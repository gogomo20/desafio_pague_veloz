namespace DesafioPagueVeloz.Persistense.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveAsync(CancellationToken cancellationToken = default);
    int Save();
}