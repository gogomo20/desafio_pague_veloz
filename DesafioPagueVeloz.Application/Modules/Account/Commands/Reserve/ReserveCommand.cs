using DesafioPagueVeloz.Application.Modules.Account.Responses;
using DesafioPagueVeloz.Application.Response;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;
using MediatR;

namespace DesafioPagueVeloz.Application;

public record ReserveCommand(
    decimal Value,
    string Description,
    string Currency
) : IRequest<GenericResponse<OperationResponse>>
{
    public Guid AccountId { get; set; }
}

public record ReserveCommandHandler : IRequestHandler<ReserveCommand, GenericResponse<OperationResponse>>
{
    private readonly IReadableRepository<Account> _repository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReserveCommandHandler(
        IReadableRepository<Account> repository,
        ICurrencyRepository currencyRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currencyRepository = currencyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GenericResponse<OperationResponse>> Handle(ReserveCommand request,
        CancellationToken cancellationToken)
    {
        var response = new GenericResponse<OperationResponse>();
        var account = await _repository.GetByIdAsync(request.AccountId);
        if (account is null)
            throw AppException.NotFound("A conta informada não existe");
        var currency = await _currencyRepository.GetAsync(request.Currency);
        if (currency is null)
            throw AppException.NotFound("Não possuimos essa moeda em nossa base");
        var percentDirferecent = currency.Price / account.Currency.Price;
        if (account.Balance < (request.Value * percentDirferecent))
            throw AppException.Invalid("Saldo insuficiente");
        var operation = new Operation(
            OperationType.reserve,
            currency!,
            request.Description,
            request.Value
        );
        account.AddOperation(operation);
        await _unitOfWork.SaveAsync(cancellationToken);
        response.SetSuccess(new OperationResponse(operation.Id), "Operação solicitada com sucesso!");
        return response;
    }
}