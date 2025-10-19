using DesafioPagueVeloz.Domain.DomainExceptions;

namespace DesafioPagueVeloz.Domain.Entities;

public class Currency : BaseEntity
{
    public string Name { get; private set; }
    public string Code { get; private set; }
    public decimal Price { get; private set; }

    public Currency(string name, string code)
    {
        Name = name;
        Code = code;
        Price = 0;
    }
    public Currency(string name, string code, decimal price)
    {
        Name = name;
        Code = code;
        Price = price;
    }

    public void SetPrice(decimal price)
    {
        if (price <= 0)
            throw new DomainException("A cotação atual da moeda precia ser maior ou igual a zero");
        Price = price;
    }
    private Currency(){}
}