using DesafioPagueVeloz.Application.Modules.Account.Responses;
using DesafioPagueVeloz.Application.Response;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;
using MediatR;

namespace DesafioPagueVeloz.Application;

public record ReverseCommand(
    Guid OperationId
) : IRequest<GenericResponse<OperationResponse>>
{
    public Guid AccountId { get; set; }
}

public record ReverseCommandHandler : IRequestHandler<ReverseCommand, GenericResponse<OperationResponse>>
{
    private readonly IReadableRepository<Account> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ReverseCommandHandler(IReadableRepository<Account> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GenericResponse<OperationResponse>> Handle(ReverseCommand request,
        CancellationToken cancellationToken)
    {
        var response = new GenericResponse<OperationResponse>();
        var account = await _repository.GetByIdAsync(request.AccountId);
        if (account is null)
            throw AppException.NotFound("A conta informada não existe");
        var undoOperation = account.Operations.FirstOrDefault(o => o.Id == request.OperationId);
        if (undoOperation is null)
            throw AppException.NotFound("A operação informada não existe");
        if (undoOperation.Status != OperationStatus.completed)
            throw AppException.Invalid("A operação informada não pode ser desfeita pois ela ainda não foi processada");
        if (account.Operations.Any(x => x.OperationType == OperationType.reverse && x.Undo?.Id == request.OperationId))
            throw AppException.Invalid("Já existe uma solicitação registrada para desfazer a operação informada");

        var operation = undoOperation.UndoOperation();
        account.AddOperation(operation);
        await _unitOfWork.SaveAsync(cancellationToken);
        response.SetSuccess(new OperationResponse(operation.Id), "Operação solicitada com sucesso!");
        return response;
    }
}