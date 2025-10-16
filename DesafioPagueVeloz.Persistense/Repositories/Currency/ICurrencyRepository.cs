using DesafioPagueVeloz.Domain.Entities;

namespace DesafioPagueVeloz.Persistense.Repositories;

public interface ICurrencyRepository
{
    Task<Currency?> GetAsync(string currency);
    Task<bool> CurrencyExistsAsync(string currency);
}