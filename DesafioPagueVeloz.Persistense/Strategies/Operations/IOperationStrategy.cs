using DesafioPagueVeloz.Domain.Entities;

namespace DesafioPagueVeloz.Persistense.Strategies.Operations;

public interface IOperationStrategy
{
    Task ExecuteAsync(Operation operation);
}