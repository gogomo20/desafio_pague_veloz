using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;

namespace DesafioPagueVeloz.Persistense.Strategies.Operations.Transfer;

public class TransferStrategy : IOperationStrategy
{
    private readonly IReadableRepository<Account> _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransferStrategy(IReadableRepository<Account> accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Operation operation)
    {
        var account = await _accountRepository.GetByIdAsync(operation.AccountId);
        var transferAccount = await _accountRepository.GetByIdAsync((Guid)operation.TransferId!);
        if(account == null || transferAccount == null)
            operation.Cancel();
        else
        {
            var percentDiference = operation.Currency.Price / account.Currency.Price;
            operation.SetPreviousBalance(account.Balance);
            var transferValue = percentDiference * account.Currency.Price;
            account.Transfer(transferValue, transferAccount);
            operation.SetResultBalance(account.Balance);
            operation.SetAvaliableCredit(account.AvaliableCredit);
            operation.SetReservedAmount(account.ReservedAmount);
            operation.Complete();
        }
        await _unitOfWork.SaveAsync();
    }
}