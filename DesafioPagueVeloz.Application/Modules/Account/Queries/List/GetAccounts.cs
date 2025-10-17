using DesafioPagueVeloz.Application.Response;
using DesafioPagueVeloz.Persistense.Repositories;
using DesafioPagueVeloz.Persistense.Repositories.Account;
using MediatR;

namespace DesafioPagueVeloz.Application;

public record GetAccounts(string clientId) : IRequest<GenericResponse<ICollection<AccountView>>>;

public class GetAccountsHandler : IRequestHandler<GetAccounts, GenericResponse<ICollection<AccountView>>>
{
    private readonly IAccountRepository _repository;
    public GetAccountsHandler(IAccountRepository repository)
    {
        _repository = repository;
    }
    public async Task<GenericResponse<ICollection<AccountView>>> Handle(GetAccounts request, CancellationToken cancellationToken)
    {
        var accounts = await _repository.GetAccounts(request.clientId);
        return new GenericResponse<ICollection<AccountView>>(accounts, "Contas encontradas com sucesso!");
    }
}