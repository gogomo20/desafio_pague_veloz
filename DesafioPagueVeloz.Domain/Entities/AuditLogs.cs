using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace DesafioPagueVeloz.Domain.Entities;

public enum TypeActionsAuditLogs
{
    Create,
    Update,
    Delete
}
public class AuditLogs
{
    public Guid Id { get; } = Guid.NewGuid();
    public TypeActionsAuditLogs Action { get; private set; }
    public string Entity { get; private set; }

    [Column(TypeName = "jsonb")]
    public string? OldValue { get; private set; }

    [Column(TypeName = "jsonb")]
    public string? NewValue { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public AuditLogs(TypeActionsAuditLogs action, string entity, string? oldValue, string? newValue)
    {
        Action = action;
        Entity = entity;

        // 🔧 Garantir sempre JSON válido
        OldValue = EnsureValidJson(oldValue);
        NewValue = EnsureValidJson(newValue);
    }

    private static string EnsureValidJson(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "{}";

        try
        {
            JsonDocument.Parse(value);
            return value; // é um JSON válido
        }
        catch
        {
            // se não for um JSON válido, serializa como texto bruto
            return JsonSerializer.Serialize(value);
        }
    }
}