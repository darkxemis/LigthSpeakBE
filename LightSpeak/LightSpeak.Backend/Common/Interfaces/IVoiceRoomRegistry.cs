namespace LightSpeak.Backend.Common.Interfaces;

using LightSpeak.Backend.Dominio;

public sealed record VoicePeer(
    string ConnectionId,
    Guid UserId,
    string Username,
    bool IsMuted = false,
    bool IsSpeaking = false,
    bool IsDeafened = false);

public interface IVoiceRoomRegistry
{
    IReadOnlyList<VoicePeer> Join(Guid channelId, string connectionId, Guid userId, string username);

    void Leave(Guid channelId, string connectionId);

    void RemoveConnection(string connectionId);

    bool AreInSameRoom(string sourceConnectionId, string targetConnectionId);

    IReadOnlyList<VoicePeer> GetChannelPeers(Guid channelId);

    IReadOnlyList<Guid> GetChannelsForConnection(string connectionId);

    void UpdateVoiceState(string connectionId, bool isSpeaking, bool isMuted, bool isDeafened);
}
