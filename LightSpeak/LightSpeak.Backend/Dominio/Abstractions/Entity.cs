namespace LightSpeak.Backend.Dominio;

public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public Guid? CreatedByUserId { get; protected set; }
    public Guid? UpdatedByUserId { get; protected set; }
}
