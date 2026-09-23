namespace LightSpeak.Backend.Dominio;

public sealed class Message : Entity
{
    public Guid ChannelId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Content { get; private set; } = default!;
    public DateTime? EditedAt { get; private set; }

    public Channel Channel { get; private set; } = default!;
    public User Author { get; private set; } = default!;

    private Message() { }

    public static Message Create(Guid channelId, Guid authorId, string content)
    {
        return new Message
        {
            ChannelId = channelId,
            AuthorId = authorId,
            Content = content
        };
    }

    public void Edit(string content)
    {
        Content = content;
        EditedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
