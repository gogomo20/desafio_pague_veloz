namespace DesafioPagueVeloz.Persistense.Repositories.Account;

public interface IAccountRepository
{
    public Task<AccountView?> GetAccount(Guid id);
    public Task<ICollection<HistoryView>?> GetHistory(Guid id);
    public Task<ICollection<AccountView>> GetAccounts(string clientId);
}