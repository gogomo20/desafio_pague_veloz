using System.Runtime.InteropServices.JavaScript;
using DesafioPagueVeloz.Domain.DomainExceptions;
using DesafioPagueVeloz.Domain.Entities.Accounts;

namespace DesafioPagueVeloz.Domain.Entities;

public enum OperationType
{
    debit,
    reserve,
    transfer,
    credit,
    capture,
    reverse
}
public enum OperationStatus
{
    pending,
    completed,
    canceled,
    undone,
    error
}

public class Operation : BaseEntity
{
    public OperationType OperationType { get; private set; }
    public decimal Value { get; private set; } 
    public OperationStatus Status { get; private set; }
    public Guid AccountId { get; private set;  }
    public Account Account { get; private set; }
    public Guid? TransferId { get; private set; }
    public Account? TransferAccount { get; private set; }
    public Currency Currency { get; private set; }
    public string Description { get; private set; }
    public decimal? PreviousBalance { get; private set; }
    public decimal? ResultBalance { get; private set; }
    public decimal? ReservedAmount { get; private set; }
    public decimal? AvaliableCredit { get; private set; }
    public Operation? Undo { get; private set; }
    public int RetryCounter { get; private set; } = 0;
    public Guid? ErrorId { get; private set; }
    public ErrorLogs? Error { get; private set; }
    public bool IsReversable { get; private set; } = true;
    public DateTime? ExecutionDate { get; private set; }
    public Operation(OperationType operationType, Currency currency, string description, decimal value)
    {
        var acceptedOperations = new[]
            { OperationType.debit, OperationType.credit, OperationType.capture, OperationType.reserve };
        if (!acceptedOperations.Contains(operationType))
            throw new DomainException("A movimentação solictada não é possivel ser realizada");
        OperationType = operationType;
        Value = value;
        Currency = currency;
        Status = OperationStatus.pending;
        Description = description;
    }
    public Operation(OperationType operationType, Currency currency, string description, decimal value, Account to)
    {
        if(operationType != OperationType.transfer)
            throw new DomainException("A movimentação solictada não é possivel ser realizada");
        OperationType = operationType;
        Value = value;
        Currency = currency;
        Status = OperationStatus.pending;
        TransferAccount = to;
        Description = description;
        IsReversable = false;
    }
    private Operation(OperationType operationType, Operation undo)
    {
        if (operationType != OperationType.reverse && undo.Status != OperationStatus.completed)
            throw new DomainException("A operação solicitada é invalida");
        if(!undo.IsReversable)
            throw new DomainException("A operação solicitada não pode ser revertida");
        OperationType = operationType;
        Value = undo.Value;
        Currency = undo.Currency;
        Undo = undo;
        Status = OperationStatus.pending;
        Description = undo.Description;
    }
    public void SetPreviousBalance(decimal previousBalance)
    {
        if(Status == OperationStatus.completed || Status == OperationStatus.canceled)
            throw new DomainException("Operação ja foi concluida ou cancelada");
        PreviousBalance = previousBalance;
    }

    public void SetResultBalance(decimal resultBalance)
    {
        if(Status == OperationStatus.completed || Status == OperationStatus.canceled)
            throw new DomainException("Operação ja foi concluida ou cancelada");
        ResultBalance = resultBalance;
    }

    public void SetReservedAmount(decimal reservedAmount)
    {
        if(Status == OperationStatus.completed || Status == OperationStatus.canceled)
            throw new DomainException("Operação ja foi concluida ou cancelada");
        ReservedAmount = reservedAmount;
    }
    public void SetAvaliableCredit(decimal avaliableCredit)
    {
        if(Status == OperationStatus.completed || Status == OperationStatus.canceled)
            throw new DomainException("Operação ja foi concluida ou cancelada");
        AvaliableCredit = avaliableCredit;
    }

    public void Complete()
    {
        ExecutionDate = DateTime.UtcNow;
        Status = OperationStatus.completed;
    }

    public void Cancel(string reason)
    {
        if (string.IsNullOrEmpty(reason))
            throw new DomainException("A rasão pelo cancelamento da operação precisa ser informado");
        Description = reason;
        Status = OperationStatus.canceled;
    }

    public void SetError(ErrorLogs error)
    {
        Error = error;
        Status = OperationStatus.error;
    }

    public void PushCounter()
    {
        RetryCounter++;
    }
    public Operation UndoOperation()
    {
        if (Status != OperationStatus.completed)
            throw new DomainException("Não é possivel desfazer a operação pois a ação não foi processada");
        return new Operation(OperationType.reverse, this);
    }
    public void SetIrreversible()
    {
        IsReversable = false;
    }
    private Operation(){}

    //Usar somente em testes para não gerar conflitos
    public void GenerateId()
    {
        Id = Guid.NewGuid();
    }
}