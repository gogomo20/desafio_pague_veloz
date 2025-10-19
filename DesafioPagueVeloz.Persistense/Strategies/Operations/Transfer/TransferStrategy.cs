using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;

namespace DesafioPagueVeloz.Persistense.Strategies.Operations.Transfer;

public class TransferStrategy : IOperationStrategy
{
    private readonly IReadableRepository<Operation> _repository;
    public TransferStrategy(IReadableRepository<Operation> repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Operation operation)
    {
        await _repository.LoadProperty(operation, x => x.Account);
        await _repository.LoadProperty(operation, x => x.TransferAccount);
        if (operation.TransferAccount is null)
        {
            operation.Cancel("Operação inválida");
            return;
        }

        try
        {
            var percentDiference = operation.Currency.Price / operation.Account.Currency.Price;
            operation.SetPreviousBalance(operation.Account.Balance);
            var transferValue = percentDiference * operation.Account.Currency.Price;
            operation.Account.Transfer(transferValue, operation.TransferAccount);
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