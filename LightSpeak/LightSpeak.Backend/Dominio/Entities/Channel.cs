namespace LightSpeak.Backend.Dominio;

public sealed class Channel : Entity
{
    public Guid ServerId { get; private set; }
    public string Name { get; private set; } = default!;
    public ChannelType Type { get; private set; }
    public int Position { get; private set; }

    public Server Server { get; private set; } = default!;
    public ICollection<Message> Messages { get; private set; } = new List<Message>();

    private Channel() { }

    public static Channel Create(Guid serverId, string name, ChannelType type, int position)
    {
        return new Channel
        {
            ServerId = serverId,
            Name = name,
            Type = type,
            Position = position
        };
    }
}
