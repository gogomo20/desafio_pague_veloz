namespace DesafioPagueVeloz.Domain.Entities.Accounts;

public class Account : BaseEntity
{
    public decimal Balance { get; private set; }
    public decimal ReservedAmount { get; private set; }
    public Currency Currency { get; }
    public decimal CreditLimit { get; }

    public Account(
        decimal balance,
        Currency currency,
        decimal creditLimit,
        decimal reservedAmount
    )
    {
        if(balance < 0)
            throw new ArgumentException("Saldo inicial deve ser maior ou igual a zero");
        Balance = balance;
        if(currency == null)
            throw new ArgumentNullException("Moeda deve ser informada");
        Currency = currency;
        if(creditLimit < 0)
            throw new ArgumentException("Limite de crédito deve ser maior ou igual a zero");
        CreditLimit = creditLimit;
        if(reservedAmount < 0)
            throw new ArgumentException("Valor reservado deve ser maior ou igual a zero");
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
        to.Balance += value / to.Currency.Price;
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
        Balance -= value; 
    }
}