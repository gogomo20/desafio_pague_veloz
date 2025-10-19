using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;

namespace DesafioPagueVeloz.Persistense.Strategies.Operations.Credit;

public class CreditStrategy : IOperationStrategy
{
    private readonly IReadableRepository<Operation> _repository;

    public CreditStrategy(IReadableRepository<Operation> repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Operation operation)
    {
        await _repository.LoadProperty(operation, x => x.Account);
        var pricePercent = operation.Currency.Price / operation.Account.Currency.Price;
        operation.SetPreviousBalance(operation.Account.Balance);
        operation.Account.Credit(operation.Value * pricePercent);
        operation.SetResultBalance(operation.Account.Balance);
        operation.SetAvaliableCredit(operation.Account.AvaliableCredit);
        operation.SetReservedAmount(operation.Account.ReservedAmount);
        operation.Complete();
    }
}