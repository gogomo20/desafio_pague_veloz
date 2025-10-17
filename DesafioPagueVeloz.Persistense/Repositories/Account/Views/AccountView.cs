namespace DesafioPagueVeloz.Persistense.Repositories;

public class AccountView
{
    public Guid Id { get; init; }
    public required string ClientId { get; init; }
    public decimal Balance { get; init; }
    public decimal CreditLimit { get; init; }
    public decimal ReservedAmount { get; init; }
    public required string Currency { get; init; }
}