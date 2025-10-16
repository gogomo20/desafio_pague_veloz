

using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Persistense.Context;
using Microsoft.EntityFrameworkCore;

namespace DesafioPagueVeloz.Persistense.Repositories;

public class ReadableRepository<T> : IReadableRepository<T> where T : class
{
    private readonly ApplicationContext _context;
    public ReadableRepository(ApplicationContext context)
    {
        _context = context;
    }
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }
}