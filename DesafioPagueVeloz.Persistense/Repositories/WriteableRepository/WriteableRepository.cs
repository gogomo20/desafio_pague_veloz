using DesafioPagueVeloz.Persistense.Context;

namespace DesafioPagueVeloz.Persistense.Repositories;

public class WriteableRepository<T> : IWriteableRepository<T> where T : class
{
    private readonly ApplicationContext _context;
    public WriteableRepository(ApplicationContext context)
    {
        _context = context;
    }
    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }
    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _context.Set<T>().AddRangeAsync(entities);
    }
    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
    public void RemoveRange(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
    }
}