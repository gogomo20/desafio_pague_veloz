using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Context;
using DesafioPagueVeloz.Persistense.Repositories;
using DesafioPagueVeloz.Persistense.Repositories.Account;
using Microsoft.EntityFrameworkCore;

namespace DesafioPagueVeloz.Persistense;

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationContext _context;

    public AccountRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<AccountView?> GetAccount(Guid id)
    {
        var account = _context.Set<Account>().AsNoTracking();
        var query = account.Select(a => new AccountView
        {
            Id = a.Id,
            ClientId = a.ClientId,
            Balance = a.Balance,
            CreditLimit = a.CreditLimit,
            ReservedAmount = a.ReservedAmount,
            Currency = a.Currency.Name
        });
        return await query.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<ICollection<HistoryView>?> GetHistory(Guid id)
    {
        var account = _context.Set<Account>().AsNoTracking();
        var query = account
            .Where(x => x.Id == id)
            .Select(a => a.Operations
                .Select(o => new HistoryView
                {
                    Id = o.Id,
                    Status = o.Status,
                    Type = o.OperationType,
                    Value = o.Value,
                    PreviousBalance = o.PreviousBalance ?? 0,
                    ResultBalance =  o.ResultBalance ?? 0,
                    Description = o.Description
                }).ToList());
        return await query.FirstOrDefaultAsync();
    }

    public async Task<ICollection<AccountView>> GetAccounts(string clientId)
    {
        var account = _context.Set<Account>().AsNoTracking();
        var query = account.Where(x => x.ClientId == clientId)
            .Select(a => new AccountView
            {
                Id = a.Id,
                ClientId = a.ClientId,
                Balance = a.Balance,
                CreditLimit = a.CreditLimit,
                ReservedAmount = a.ReservedAmount,
                Currency = a.Currency.Name
            });
        return await query.ToListAsync();
    }
}