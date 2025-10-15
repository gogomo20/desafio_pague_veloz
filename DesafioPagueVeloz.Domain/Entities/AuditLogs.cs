using System.ComponentModel.DataAnnotations.Schema;

namespace DesafioPagueVeloz.Domain.Entities;

public enum TypeActionsAuditLogs
{
    Create,
    Update,
    Delete
}
public class AuditLogs(
    TypeActionsAuditLogs action,
    string entity,
    string oldValue,
    string newValue)
{
    public Guid Id { get; } = Guid.NewGuid();
    public TypeActionsAuditLogs Action { get; private set; } = action;
    public string Entity { get; private set; } = entity;

    [Column(TypeName = "jsonb")]
    public string? OldValue { get; private set; } = oldValue;

    [Column(TypeName = "jsonb")]
    public string? NewValue { get; private set; } = newValue;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}