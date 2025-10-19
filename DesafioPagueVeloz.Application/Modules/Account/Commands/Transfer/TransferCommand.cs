using DesafioPagueVeloz.Application.Modules.Account.Responses;
using DesafioPagueVeloz.Application.Response;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;
using MediatR;

namespace DesafioPagueVeloz.Application;

public record TransferCommand(
    Guid To,
    decimal Value,
    string Currency,
    string Description
) : IRequest<GenericResponse<OperationResponse>>
{
    public Guid AccountId { get; set; }
}

public class TransferCommandHandler : IRequestHandler<TransferCommand, GenericResponse<OperationResponse>>
{
    private readonly IReadableRepository<Account> _repository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransferCommandHandler(IReadableRepository<Account> repository, ICurrencyRepository currencyRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currencyRepository = currencyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GenericResponse<OperationResponse>> Handle(TransferCommand request,
        CancellationToken cancellationToken)
    {
        var response = new GenericResponse<OperationResponse>();
        var account = await _repository.GetByIdAsync(request.AccountId);
        var to = await _repository.GetByIdAsync(request.To);
        var currency = await _currencyRepository.GetAsync(request.Currency);
        if (account is null)
            throw AppException.NotFound("A conta informada não exite");
        if (to is null)
            throw AppException.NotFound("A conta no qual deseja realizar a transferência não existe");
        if (currency is null)
            throw AppException.NotFound("Não possuimos essa moeda em nossa base");
        var percentDirferecent = currency.Price / account.Currency.Price;
        if (account.Balance < (request.Value * percentDirferecent))
            throw AppException.Invalid("Saldo insuficiente");    
        var operation = new Operation(
            OperationType.transfer,
            currency!,
            request.Description,
            request.Value,
            to!
        );
        account!.AddOperation(operation);
        await _unitOfWork.SaveAsync(cancellationToken);
        response.SetSuccess(new OperationResponse(operation.Id),
            "Solicitação de transferência solicitada com sucesso!");
        return response;
    }
}