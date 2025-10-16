using System.ComponentModel.DataAnnotations;
using DesafioPagueVeloz.Application.Exceptions;
using DesafioPagueVeloz.Application.Modules.Account.Responses;
using DesafioPagueVeloz.Application.Response;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;
using MediatR;

namespace DesafioPagueVeloz.Application;

public record CreateAccountCommand(
    string ClientId,
    decimal Balance,
    string Currency,
    decimal CreditLimit,
    decimal ReservedAmount
) : IRequest<GenericResponse<AccountCreatedResponse>>;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, GenericResponse<AccountCreatedResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWriteableRepository<Account> _repository;
    private readonly ICurrencyRepository _repositoryCurrency;
    public CreateAccountCommandHandler(IUnitOfWork unitOfWork, IWriteableRepository<Account> repository, ICurrencyRepository repositoryCurrency)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _repositoryCurrency = repositoryCurrency;
    }
    public async Task<GenericResponse<AccountCreatedResponse>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var currency = await _repositoryCurrency.GetAsync(request.Currency);
        if(currency is null)
            throw new ValidationException("Moeda informada não existe");
        var account = new Account(
            request.ClientId,
            request.Balance,
            currency!,
            request.CreditLimit,
            request.ReservedAmount
        );
        await _repository.AddAsync(account);
        await _unitOfWork.SaveAsync(cancellationToken);
        var data = new AccountCreatedResponse(
            account.Id,
            request.Balance,
            request.CreditLimit,
            request.Currency
        );
        var response = new GenericResponse<AccountCreatedResponse>();
        response.SetSuccess(data, "Conta criada com sucesso");
        return response;
    }
}