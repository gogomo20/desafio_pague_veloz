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

    public void SetPrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("A cotação atual da moeda precia ser maior ou igual a zero");
        Price = price;
    }
}