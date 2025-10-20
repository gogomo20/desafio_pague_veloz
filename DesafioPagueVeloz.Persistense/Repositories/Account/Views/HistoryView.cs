using DesafioPagueVeloz.Domain.Entities;

namespace DesafioPagueVeloz.Persistense.Repositories;

public class HistoryView
{
    public Guid Id { get; init; }
    public OperationStatus Status { get; init; }
    public OperationType Type { get; init; }
    public decimal Value { get; init; }
    public decimal PreviousBalance { get; init; }
    public decimal ResultBalance { get; init; }
    public required string Description { get; init; }
    public DateTime? DateTime { get; init; }
}