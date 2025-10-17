using DesafioPagueVeloz.Application.Response;
using DesafioPagueVeloz.Persistense.Repositories;
using DesafioPagueVeloz.Persistense.Repositories.Account;
using MediatR;

namespace DesafioPagueVeloz.Application;

public record GetHistory(Guid id) : IRequest<GenericResponse<ICollection<HistoryView>>>;

public class GetHistoryHandler : IRequestHandler<GetHistory, GenericResponse<ICollection<HistoryView>>>
{
    private readonly IAccountRepository _repository;
    public GetHistoryHandler(IAccountRepository repository)
    {
        _repository = repository;
    }
    public async Task<GenericResponse<ICollection<HistoryView>>> Handle(GetHistory request, CancellationToken cancellationToken)
    {
        var history = await _repository.GetHistory(request.id);
        if(history is null)
            AppException.NotFound("A conta informada não existe");
        return new GenericResponse<ICollection<HistoryView>>(history, "Histórico encontrado com sucesso!");
    }
}