using System.Security.AccessControl;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;

namespace DesafioPagueVeloz.Persistense.Strategies.Operations.Capture;

public class CaptureStrategy : IOperationStrategy
{
    private readonly IReadableRepository<Operation> _repository;

    public CaptureStrategy(IReadableRepository<Operation> repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Operation operation)
    {
        try
        {
            await _repository.LoadProperty(operation, x => x.Account);
            var percentDiference = operation.Account.Currency.Price / operation.Currency.Price;
            operation.SetPreviousBalance(operation.Account.Balance);
            operation.Account.Capture(operation.Value * percentDiference);
            operation.SetResultBalance(operation.Account.Balance);
            operation.SetAvaliableCredit(operation.Account.AvaliableCredit);
            operation.SetReservedAmount(operation.Account.ReservedAmount);
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