namespace LightSpeak.Backend.Dominio;

public sealed class ServerMember
{
    public Guid ServerId { get; private set; }
    public Guid UserId { get; private set; }
    public ServerRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public Server Server { get; private set; } = default!;
    public User User { get; private set; } = default!;

    private ServerMember() { }

    public static ServerMember Create(Guid serverId, Guid userId, ServerRole role)
    {
        return new ServerMember
        {
            ServerId = serverId,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };
    }

    public void SetRole(ServerRole role)
    {
        Role = role;
    }
}
