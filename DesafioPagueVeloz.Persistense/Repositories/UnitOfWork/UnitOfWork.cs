using System.Text.Json;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Persistense.Context;
using Microsoft.EntityFrameworkCore;

namespace DesafioPagueVeloz.Persistense.Repositories;

public class UnitOfWork(ApplicationContext context) : IUnitOfWork
{
    public void ClearChanges()
    {
        context.ChangeTracker.Clear();
    }

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
        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };
        var changes = context.ChangeTracker.Entries<BaseEntity>()
            .Where(x => x.Entity is BaseEntity).ToList();
        foreach (var change in changes )
        {
            string? oldValue = null;
            string? newValue = null;
            TypeActionsAuditLogs action = TypeActionsAuditLogs.Create;
            switch (change.State)
            {
                case EntityState.Added:
                    newValue = JsonSerializer.Serialize(change.CurrentValues.ToObject(), jsonOptions);
                    break;
                case EntityState.Modified:
                    action = TypeActionsAuditLogs.Update;
                    oldValue = JsonSerializer.Serialize(change.OriginalValues.ToObject(), jsonOptions);
                    newValue = JsonSerializer.Serialize(change.CurrentValues.ToObject(), jsonOptions);
                    break;
                case EntityState.Deleted:
                    action = TypeActionsAuditLogs.Delete;
                    oldValue = JsonSerializer.Serialize(change.OriginalValues.ToObject(), jsonOptions);
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