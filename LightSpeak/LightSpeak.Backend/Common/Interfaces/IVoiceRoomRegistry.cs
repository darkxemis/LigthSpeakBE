namespace LightSpeak.Backend.Common.Interfaces;

using LightSpeak.Backend.Dominio;

public sealed record VoicePeer(string ConnectionId, Guid UserId, string Username);

public interface IVoiceRoomRegistry
{
    IReadOnlyList<VoicePeer> Join(Guid channelId, string connectionId, Guid userId, string username);

    void Leave(Guid channelId, string connectionId);

    void RemoveConnection(string connectionId);

    bool AreInSameRoom(string sourceConnectionId, string targetConnectionId);
}
