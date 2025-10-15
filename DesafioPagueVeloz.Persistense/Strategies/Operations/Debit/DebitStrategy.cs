using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;

namespace DesafioPagueVeloz.Persistense.Strategies.Operations.Debit;

public class DebitStrategy : IOperationStrategy
{
    private readonly IReadableRepository<Account> _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DebitStrategy(IReadableRepository<Account> accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task ExecuteAsync(Operation operation)
    {
        var account = await _accountRepository.GetByIdAsync(operation.AccountId);
        if (account is null)
        {
            operation.Cancel();
        }
        else
        {
            var percentDiference = operation.Currency.Price / account.Currency.Price;
            operation.SetPreviousBalance(account.Balance);
            account.Debit(operation.Value * percentDiference);
            operation.SetResultBalance(account.Balance);
            operation.SetAvaliableCredit(account.AvaliableCredit);
            operation.SetReservedAmount(account.ReservedAmount);
            operation.Complete();
        }
        await _unitOfWork.SaveAsync();
    }
}