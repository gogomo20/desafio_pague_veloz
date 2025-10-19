using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;

namespace DesafioPagueVeloz.Persistense.Strategies.Operations.Reverse;

public class ReverseStrategy : IOperationStrategy
{
    private readonly IReadableRepository<Operation> _repository;

    public ReverseStrategy(IReadableRepository<Operation> repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Operation operation)
    {
        await _repository.LoadProperty(operation, x => x.Undo);
        await _repository.LoadProperty(operation, x => x.Account);
        if (operation.Undo is null)
        {
            operation.Cancel("Operação inválida");
            return;
        }
        Operation? operationReverse = null;
        switch (operation.Undo!.OperationType)
        {
            case OperationType.debit:
                operationReverse = new Operation(
                    OperationType.credit,
                    operation.Undo.Currency,
                    "Reversão de débito",
                    operation.Undo.Value
                );
                break;
            case OperationType.credit:
                operationReverse = new Operation(
                    OperationType.debit,
                    operation.Undo.Currency,
                    "Reversão de crédito",
                    operation.Undo.Value
                );
                break;
            case OperationType.capture:
                operationReverse = new Operation(
                    OperationType.reserve,
                    operation.Undo.Currency,
                    "Reversão de captura",
                    operation.Undo.Value
                );
                break;
            case OperationType.reserve:
                operationReverse = new Operation(
                    OperationType.capture,
                    operation.Undo.Currency,
                    "Reversão de reserva",
                    operation.Undo.Value
                );
                break;
            default:
                operation.Cancel("Operação inválida");
                break;
        }

        if (operationReverse is not null)
        {
            operation.Account.AddOperation(operationReverse);
            operation.Complete();
        }
    }
}