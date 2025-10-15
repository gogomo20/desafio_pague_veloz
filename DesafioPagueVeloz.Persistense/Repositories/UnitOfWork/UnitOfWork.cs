using System.Text.Json;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Persistense.Context;
using Microsoft.EntityFrameworkCore;

namespace DesafioPagueVeloz.Persistense.Repositories;

public class UnitOfWork(ApplicationContext context) : IUnitOfWork
{
    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        VerifyChanges();
        return await context.SaveChangesAsync(cancellationToken);
    }

    public int Save()
    {
        VerifyChanges();
        return context.SaveChanges(); 
    }

    private void VerifyChanges()
    {
        var changes = context.ChangeTracker.Entries<BaseEntity>()
            .Where(x => x.Entity is BaseEntity);
        foreach (var change in changes )
        {
            string? oldValue = null;
            string? newValue = null;
            TypeActionsAuditLogs action = TypeActionsAuditLogs.Create;
            switch (change.State)
            {
                case EntityState.Added:
                    newValue = JsonSerializer.Serialize(change.CurrentValues.ToObject());
                    break;
                case EntityState.Modified:
                    action = TypeActionsAuditLogs.Update;
                    oldValue = JsonSerializer.Serialize(change.OriginalValues.ToObject());
                    newValue = JsonSerializer.Serialize(change.CurrentValues.ToObject());
                    break;
                case EntityState.Deleted:
                    action = TypeActionsAuditLogs.Delete;
                    oldValue = JsonSerializer.Serialize(change.OriginalValues.ToObject());
                    change.State = EntityState.Modified;
                    change.Entity.Deactivate();
                    break;
            }

            if (oldValue != null || newValue != null)
            {
                var log = new AuditLogs(
                    action,
                    change.Entity.GetType().Name,
                    oldValue ?? "",
                    newValue ?? ""
                );
                context.Set<AuditLogs>().Add(log);
            }
        }
    }
}