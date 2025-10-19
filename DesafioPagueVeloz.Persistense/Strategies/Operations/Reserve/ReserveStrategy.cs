using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;

namespace DesafioPagueVeloz.Persistense.Strategies.Operations.Reserve;

public class ReserveStrategy : IOperationStrategy
{
    private readonly IReadableRepository<Operation> _repository;

    public ReserveStrategy(IReadableRepository<Operation> repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Operation operation)
    {
        await _repository.LoadProperty(operation, x => x.Account);
        var percentDiference = operation.Account.Currency.Price / operation.Currency.Price;
        operation.SetPreviousBalance(operation.Account.Balance);
        operation.Account.Reserve(operation.Value * percentDiference);
        operation.SetResultBalance(operation.Account.Balance);
        operation.SetAvaliableCredit(operation.Account.AvaliableCredit);
        operation.SetReservedAmount(operation.Account.ReservedAmount);
        operation.Complete();
    }
}