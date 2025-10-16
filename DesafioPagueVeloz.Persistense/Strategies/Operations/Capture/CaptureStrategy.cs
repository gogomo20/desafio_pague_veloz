using System.Security.AccessControl;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;

namespace DesafioPagueVeloz.Persistense.Strategies.Operations.Capture;

public class CaptureStrategy : IOperationStrategy
{
    private readonly IReadableRepository<Account> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CaptureStrategy(IReadableRepository<Account> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Operation operation)
    {
        var account = await _repository.GetByIdAsync(operation.AccountId);
        if (account == null)
        {
            operation.Cancel();
        }
        else
        {
            var percentDiference = account.Currency.Price / operation.Currency.Price;
            account.Capture(operation.Value * percentDiference);
        }
        await _unitOfWork.SaveAsync();
    }
}