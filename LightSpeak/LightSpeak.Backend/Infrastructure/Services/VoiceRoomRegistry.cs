namespace LightSpeak.Backend.Infrastructure.Services;

using LightSpeak.Backend.Common.Interfaces;

public sealed class VoiceRoomRegistry : IVoiceRoomRegistry
{
    private readonly Dictionary<Guid, Dictionary<string, VoicePeer>> _rooms = new();
    private readonly object _lock = new();

    public IReadOnlyList<VoicePeer> Join(
        Guid channelId,
        string connectionId,
        Guid userId,
        string username)
    {
        lock (_lock)
        {
            if (!_rooms.TryGetValue(channelId, out var room))
            {
                room = new Dictionary<string, VoicePeer>();
                _rooms[channelId] = room;
            }

            var existingPeers = room.Values
                .Where(peer => peer.ConnectionId != connectionId)
                .ToList();

            room[connectionId] = new VoicePeer(connectionId, userId, username);
            return existingPeers;
        }
    }

    public void Leave(Guid channelId, string connectionId)
    {
        lock (_lock)
        {
            if (!_rooms.TryGetValue(channelId, out var room))
            {
                return;
            }

            room.Remove(connectionId);

            if (room.Count == 0)
            {
                _rooms.Remove(channelId);
            }
        }
    }

    public void RemoveConnection(string connectionId)
    {
        lock (_lock)
        {
            var emptyChannels = new List<Guid>();

            foreach (var (channelId, room) in _rooms)
            {
                room.Remove(connectionId);

                if (room.Count == 0)
                {
                    emptyChannels.Add(channelId);
                }
            }

            foreach (var channelId in emptyChannels)
            {
                _rooms.Remove(channelId);
            }
        }
    }

    public bool AreInSameRoom(string sourceConnectionId, string targetConnectionId)
    {
        lock (_lock)
        {
            return _rooms.Values.Any(room =>
                room.ContainsKey(sourceConnectionId) &&
                room.ContainsKey(targetConnectionId));
        }
    }

    public IReadOnlyList<VoicePeer> GetChannelPeers(Guid channelId)
    {
        lock (_lock)
        {
            if (!_rooms.TryGetValue(channelId, out var room))
            {
                return [];
            }

            return room.Values.ToList();
        }
    }

    public IReadOnlyList<Guid> GetChannelsForConnection(string connectionId)
    {
        lock (_lock)
        {
            return _rooms
                .Where(kv => kv.Value.ContainsKey(connectionId))
                .Select(kv => kv.Key)
                .ToList();
        }
    }

    public void UpdateVoiceState(string connectionId, bool isSpeaking, bool isMuted, bool isDeafened)
    {
        lock (_lock)
        {
            foreach (var room in _rooms.Values)
            {
                if (room.TryGetValue(connectionId, out var peer))
                {
                    room[connectionId] = peer with
                    {
                        IsSpeaking = isSpeaking,
                        IsMuted = isMuted,
                        IsDeafened = isDeafened
                    };
                }
            }
        }
    }
}
