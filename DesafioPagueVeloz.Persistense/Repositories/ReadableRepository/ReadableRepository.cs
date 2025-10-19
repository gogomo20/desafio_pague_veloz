

using System.Linq.Expressions;
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

    public async Task LoadProperty<TProperty>(T entity, Expression<Func<T, TProperty>> property)
    {
        var entry = _context.Entry(entity);
        if (property.Body is MemberExpression member)
        {
            var reference = entry.References
                .FirstOrDefault(r => r.Metadata.Name == member.Member.Name);

            if (reference != null)
            {
                if (!reference.IsLoaded)
                    await reference.LoadAsync();
                return;
            }

            var collection = entry.Collections
                .FirstOrDefault(c => c.Metadata.Name == member.Member.Name);

            if (collection != null && !collection.IsLoaded)
                await collection.LoadAsync();

            return;
        }

        throw new ArgumentException("A expressão deve ser uma propriedade válida.", nameof(property));
    }
}