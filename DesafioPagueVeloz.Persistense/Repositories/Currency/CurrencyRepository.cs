using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Persistense.Context;
using Microsoft.EntityFrameworkCore;

namespace DesafioPagueVeloz.Persistense.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly ApplicationContext _context;
    public CurrencyRepository(ApplicationContext context)
    {
        _context = context;
    }
    public async Task<Currency?> GetAsync(string currency)
    {
        return await _context.Set<Currency>().SingleOrDefaultAsync(x => x.Name == currency);
    }
    public async Task<bool> CurrencyExistsAsync(string currency)
    {
        return await _context.Set<Currency>().AnyAsync(x => x.Name == currency);
    }
}