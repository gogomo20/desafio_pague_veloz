using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;

namespace DesafioPagueVeloz.Persistense.Strategies.Operations.Reserve;

public class ReserveStrategy : IOperationStrategy
{
    private readonly IReadableRepository<Account> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ReserveStrategy(IReadableRepository<Account> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Operation operation)
    {
        var account = await _repository.GetByIdAsync(operation.AccountId);
        if(account is null)
            operation.Cancel();
        else
        {
            var percentDiference = account.Currency.Price / operation.Currency.Price;
            operation.SetPreviousBalance(account.Balance);
            account.Reserve(operation.Value * percentDiference);
            operation.SetResultBalance(account.Balance);
            operation.SetAvaliableCredit(account.AvaliableCredit);
            operation.SetReservedAmount(account.ReservedAmount);
            operation.Complete();
        }
        await _unitOfWork.SaveAsync();
    }
}