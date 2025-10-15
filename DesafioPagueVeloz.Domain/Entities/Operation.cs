using DesafioPagueVeloz.Domain.Entities.Accounts;

namespace DesafioPagueVeloz.Domain.Entities;

public enum OperationType
{
    credit,
    debit,
    transfer,
    reserve,
    capture
}
public enum OperationStatus
{
    pending,
    completed,
    canceled
}

public class Operation : BaseEntity
{
    public OperationType OperationType { get; private set; }
    public decimal Value { get; private set; } 
    public OperationStatus Status { get; private set; }
    public Guid AccountId { get; private set;  }
    public Guid? TransferId { get; private set; }
    public Currency Currency { get; private set; }
    public string Description { get; private set; }
    public decimal? PreviousBalance { get; private set; }
    public decimal? ResultBalance { get; private set; }
    public decimal? ReservedAmount { get; private set; }
    public decimal? AvaliableCredit { get; private set; }
    public Operation(OperationType operationType, Currency currency, string description, decimal value, Guid? transferId)
    {
        OperationType = operationType;
        Value = value;
        Currency = currency;
        Status = OperationStatus.pending;
        TransferId = transferId;
        Description = description;
    }
    public void SetPreviousBalance(decimal previousBalance)
    {
        if(Status == OperationStatus.completed || Status == OperationStatus.canceled)
            throw new ArgumentException("Operação ja foi concluida ou cancelada");
        PreviousBalance = previousBalance;
    }

    public void SetResultBalance(decimal resultBalance)
    {
        if(Status == OperationStatus.completed || Status == OperationStatus.canceled)
            throw new ArgumentException("Operação ja foi concluida ou cancelada");
        ResultBalance = resultBalance;
    }

    public void SetReservedAmount(decimal reservedAmount)
    {
        if(Status == OperationStatus.completed || Status == OperationStatus.canceled)
            throw new ArgumentException("Operação ja foi concluida ou cancelada");
        ReservedAmount = reservedAmount;
    }
    public void SetAvaliableCredit(decimal avaliableCredit)
    {
        if(Status == OperationStatus.completed || Status == OperationStatus.canceled)
            throw new ArgumentException("Operação ja foi concluida ou cancelada");
        AvaliableCredit = avaliableCredit;
    }

    public void Complete()
    {
        Status = OperationStatus.completed;
    }

    public void Cancel()
    {
        Status = OperationStatus.canceled;
    }
}