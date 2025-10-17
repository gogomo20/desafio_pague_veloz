namespace DesafioPagueVeloz.Application.Modules.Account.Responses;

public class OperationResponse(Guid operationId)
{
    public Guid OperationId { get; private set; } = operationId;
}