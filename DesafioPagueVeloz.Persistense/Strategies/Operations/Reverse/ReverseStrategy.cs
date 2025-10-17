using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;

namespace DesafioPagueVeloz.Persistense.Strategies.Operations.Reverse;

public class ReverseStrategy : IOperationStrategy
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IReadableRepository<Account> _accountRepository;
    public ReverseStrategy(IReadableRepository<Account> accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Operation operation)
    {
        var account = await _accountRepository.GetByIdAsync(operation.AccountId);
        if (account is null)
            operation.Cancel();
        else if (operation.Undo is null)
            operation.Cancel();
        else if (!operation.Undo.IsReversable)
            operation.Cancel();
        else
        {
            var percentDiference = operation.Currency.Price / operation.Undo.Currency.Price;
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
            }
            if(operationReverse is not null)
            {
                account.AddOperation(operationReverse);
                operationReverse.Complete();
            }      
        }

        await _unitOfWork.SaveAsync();
    }
}