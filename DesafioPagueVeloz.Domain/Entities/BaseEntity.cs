

namespace DesafioPagueVeloz.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public bool Active { get; private set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public void Deactivate() => Active = false;
}