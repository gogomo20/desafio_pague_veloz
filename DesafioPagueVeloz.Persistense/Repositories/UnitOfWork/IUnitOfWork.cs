namespace DesafioPagueVeloz.Persistense.Repositories;

public interface IUnitOfWork
{
    void ClearChanges();
    Task<int> SaveAsync(CancellationToken cancellationToken = default);
    int Save();
}