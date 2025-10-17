using DesafioPagueVeloz.Application.Response;
using DesafioPagueVeloz.Persistense.Repositories;
using DesafioPagueVeloz.Persistense.Repositories.Account;
using MediatR;

namespace DesafioPagueVeloz.Application.Modules.Account.Queries.Get;

public record GetAccount(Guid Id) : IRequest<GenericResponse<AccountView>>;

public class GetAccountHandler : IRequestHandler<GetAccount, GenericResponse<AccountView>>
{
    private readonly IAccountRepository _repository;
    public GetAccountHandler(IAccountRepository repository)
    {
        _repository = repository;
    }
    public async Task<GenericResponse<AccountView>> Handle(GetAccount request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetAccount(request.Id);
        if (account is null)
            AppException.NotFound("A conta informada não existe");
        return new GenericResponse<AccountView>(account, "Conta encontrada com sucesso!");
    }
}
