using System.Text.Json.Serialization;
using DesafioPagueVeloz.Application.Modules.Account.Responses;
using DesafioPagueVeloz.Application.Response;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;
using MediatR;

namespace DesafioPagueVeloz.Application;

public record CreditCommand(
    string Description,
    decimal Value,
    string Currency
) : IRequest<GenericResponse<OperationResponse>>
{
    public Guid AccountId { get; set; }
};

public class CreditCommandHandler : IRequestHandler<CreditCommand, GenericResponse<OperationResponse>>
{
    private readonly IReadableRepository<Account> _repository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreditCommandHandler(
        IReadableRepository<Account> repository,
        ICurrencyRepository currencyRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currencyRepository = currencyRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<GenericResponse<OperationResponse>> Handle(CreditCommand request, CancellationToken cancellationToken)
    {
        var response = new GenericResponse<OperationResponse>();
        var account = await _repository.GetByIdAsync(request.AccountId);
        var currency = await _currencyRepository.GetAsync(request.Currency);
        if (account is null)
            throw AppException.NotFound("A conta informada não exite");
        if (currency is null)
            throw AppException.NotFound("Não possuimos essa moeda em nossa base");
        var operation = new Operation(
            OperationType.credit,
            currency!,
            request.Description,
            request.Value
        );
        account.AddOperation(operation);
        await _unitOfWork.SaveAsync(cancellationToken);
        response.SetSuccess(new OperationResponse(operation.Id),"Solicitação de crédito solicitada com sucesso!");
        return response;
    }
}