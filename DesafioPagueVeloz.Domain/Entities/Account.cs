namespace DesafioPagueVeloz.Domain.Entities.Accounts;

public class Account : BaseEntity
{
    public decimal Balance { get; private set; }
    public string ClientId { get; private set; }
    public decimal ReservedAmount { get; private set; }
    public Currency Currency { get; }
    public decimal CreditLimit { get; }
    public decimal AvaliableCredit { get; private set; }
    public HashSet<Operation> Operations { get; private set; } = [];

    public Account(
        string clientId,
        decimal balance,
        Currency currency,
        decimal creditLimit,
        decimal reservedAmount
    )
    {
        if(String.IsNullOrEmpty(clientId))
            throw new ArgumentNullException("Id do cliente deve ser informado");
        if (balance > 0)
            Operations.Add(new Operation(OperationType.debit, currency, "Depósito incial", balance));
        if(currency == null)
            throw new ArgumentNullException("Moeda deve ser informada");
        ClientId = clientId;
        Currency = currency;
        CreditLimit = creditLimit;
        ReservedAmount = reservedAmount;
    }

    public void Reserve(decimal value)
    {
        if(value < 0)
            throw new ArgumentException("Valor reservado deve ser maior ou igual a zero");
        if(Balance < value)
            throw new ArgumentException("Saldo insuficiente");
        Balance -= value;
        ReservedAmount += value;
    }
    public void Transfer(decimal value, Account to)
    {
        if(value < 0)
            throw new ArgumentException("Valor transferido deve ser maior ou igual a zero");
        if(value > Balance)
            throw new ArgumentException("Saldo insuficiente");
        Balance -= value;
        var priceDirefence = Currency.Price / to.Currency.Price;
        var operationDebit = new Operation(
                OperationType.credit,
                Currency,
                $"Transferencia enviada",
                value
            );
        operationDebit.SetIrreversible();
        to.AddOperation(operationDebit);
    }
    public void Capture(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("O valor de captura deve ser maior ou igual a zero");
        if(ReservedAmount < value)
            throw new ArgumentException("Saldo reservado insuficiente");
        ReservedAmount -= value;
        Balance += value;
    }
    public void Debit(decimal value)
    {
        if ((Balance + CreditLimit) < value)
            throw new ArgumentException("Saldo insuficience!");
        Balance -= value > Balance ? Balance : value;
        AvaliableCredit -= value > Balance ? value - Balance : 0;
    }
    public void Credit(decimal value)
    {
        Balance += value;
    }
    public void AddOperation(Operation operation)
    {
        Operations.Add(operation);
    } 
    private Account(){}
}