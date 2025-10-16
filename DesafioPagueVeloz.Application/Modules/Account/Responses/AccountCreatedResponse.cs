namespace DesafioPagueVeloz.Application.Modules.Account.Responses;

public class AccountCreatedResponse
{
    public Guid AccountId { get; private set; }
    public decimal Balance { get; private set; }
    public decimal CreditLimit { get; private set; }
    public string Currency { get; private set; }

    public AccountCreatedResponse(
            Guid accountId,
            decimal balance,
            decimal creditLimit,
            string currency
        )
    {
        AccountId = accountId;
        Balance = balance;
        CreditLimit = creditLimit;
        Currency = currency;
    }
}