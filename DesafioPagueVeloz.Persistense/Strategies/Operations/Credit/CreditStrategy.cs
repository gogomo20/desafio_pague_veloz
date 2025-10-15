using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;

namespace DesafioPagueVeloz.Persistense.Strategies.Operations.Credit;

public class CreditStrategy : IOperationStrategy
{
    private readonly IReadableRepository<Account> _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreditStrategy(IReadableRepository<Account> accountRepository, IUnitOfWork unitOfWork)
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
            var pricePercent = operation.Currency.Price / account.Currency.Price;
            account.Credit(operation.Value * pricePercent);
            operation.Complete();
        }
        await _unitOfWork.SaveAsync();
    }
}