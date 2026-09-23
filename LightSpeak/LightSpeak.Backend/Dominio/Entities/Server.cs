namespace LightSpeak.Backend.Dominio;

using System.Security.Cryptography;
using System.Text;

public sealed class Server : Entity
{
    private const string InviteAlphabet = "abcdefghijkmnopqrstuvwxyz23456789";
    private const int InviteCodeLength = 10;

    public string Name { get; private set; } = default!;
    public Guid OwnerId { get; private set; }
    public string? IconUrl { get; private set; }
    public string InviteCode { get; private set; } = default!;

    public ICollection<Channel> Channels { get; private set; } = new List<Channel>();
    public ICollection<ServerMember> Members { get; private set; } = new List<ServerMember>();

    private Server() { }

    public static Server Create(string name, Guid ownerId)
    {
        return new Server
        {
            Name = name,
            OwnerId = ownerId,
            InviteCode = GenerateInviteCode()
        };
    }

    public static string GenerateInviteCode()
    {
        var builder = new StringBuilder(InviteCodeLength);
        for (var i = 0; i < InviteCodeLength; i++)
        {
            builder.Append(InviteAlphabet[RandomNumberGenerator.GetInt32(InviteAlphabet.Length)]);
        }

        return builder.ToString();
    }

    public void Rename(string name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetIcon(string? iconUrl)
    {
        IconUrl = iconUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RegenerateInviteCode()
    {
        InviteCode = GenerateInviteCode();
        UpdatedAt = DateTime.UtcNow;
    }
}
