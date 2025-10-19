using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;

namespace DesafioPagueVeloz.Persistense.Strategies.Operations.Debit;

public class DebitStrategy : IOperationStrategy
{
    private readonly IReadableRepository<Operation> _repository;

    public DebitStrategy(IReadableRepository<Operation> repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Operation operation)
    {
        await _repository.LoadProperty(operation, x => x.Account);
        var percentDiference = operation.Currency.Price / operation.Account.Currency.Price;
        try
        {
            operation.SetPreviousBalance(operation.Account.Balance);
            operation.Account.Debit(operation.Value * percentDiference);
            operation.SetResultBalance(operation.Account.Balance);
            operation.SetAvaliableCredit(operation.Account.AvaliableCredit);
            operation.SetReservedAmount(operation.Account.ReservedAmount);
            operation.Complete();
        }
        catch (Exception e)
        {
            switch (e)
            {
                case ArgumentException:
                    operation.Cancel(e.Message);
                    break;
                default:
                    throw;
            }
        }
    }
}