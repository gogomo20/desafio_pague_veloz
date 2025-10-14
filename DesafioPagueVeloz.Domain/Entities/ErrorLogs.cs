using System.ComponentModel.DataAnnotations.Schema;

namespace DesafioPagueVeloz.Domain.Entities;

public class ErrorLogs(
        string command,
        string? data,
        string message
    )
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Command { get; private set; } = command;
    [Column(TypeName = "jsonb")]
    public string? Data { get; private set; } = data;
    public string Message { get; private set; } = message;
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
}