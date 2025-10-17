using DesafioPagueVeloz.Application.Modules.Account.Responses;
using DesafioPagueVeloz.Application.Response;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;
using MediatR;

namespace DesafioPagueVeloz.Application;

public record DebitCommand(
    decimal Value,
    string Description,
    string Currency
) : IRequest<GenericResponse<OperationResponse>>
{
    public Guid AccountId { get; set; }
}

public class DebitCommandHandler : IRequestHandler<DebitCommand, GenericResponse<OperationResponse>>
{
    private readonly IReadableRepository<Account> _accountRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DebitCommandHandler(
        IReadableRepository<Account> accountRepository,
        ICurrencyRepository currencyRepository,
        IUnitOfWork unitOfWork
    )
    {
        _accountRepository = accountRepository;
        _currencyRepository = currencyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GenericResponse<OperationResponse>> Handle(DebitCommand request,
        CancellationToken cancellationToken)
    {
        var response = new GenericResponse<OperationResponse>();
        var account = await _accountRepository.GetByIdAsync(request.AccountId);
        var currency = await _currencyRepository.GetAsync(request.Currency);
        if (account is null)
            AppException.NotFound("A conta informada não exite");
        if (currency is null)
            AppException.NotFound("Não possuimos essa moeda em nossa base");
        var operation = new Operation(
            OperationType.debit,
            currency!,
            request.Description,
            request.Value
        );
        account.AddOperation(operation);
        await _unitOfWork.SaveAsync(cancellationToken);
        response.SetSuccess(new OperationResponse(operation.Id), "Solicitação de débito solicitada com sucesso!");
        return response;
    }
}