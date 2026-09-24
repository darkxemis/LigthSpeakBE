namespace LightSpeak.Backend.Features.Voice;

using System.Security.Claims;
using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

// Signaling only: relays WebRTC offers/answers/ICE candidates between peers in
// the same voice channel. Audio/video/screen-share travels P2P between clients.
[Authorize]
public sealed class VoiceHub(
    IServerMembershipService membership,
    IVoiceRoomRegistry rooms,
    IApplicationDbContext db,
    IHubContext<ChatHub> chatHub
) : Hub
{
    public async Task JoinVoiceChannel(Guid channelId)
    {
        if (!await membership.IsChannelMemberAsync(channelId, GetUserId()))
        {
            throw new HubException(Error.ChannelNotFound().Message);
        }

        var existingPeers = rooms.Join(channelId, Context.ConnectionId, GetUserId(), GetUsername());

        var userIds = existingPeers.Select(p => p.UserId).Append(GetUserId()).Distinct().ToList();
        var profileImages = await db.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.ProfileImageUrl);

        await Groups.AddToGroupAsync(Context.ConnectionId, VoiceGroup(channelId));

        await Clients.Caller.SendAsync(
            "ExistingPeers",
            existingPeers.Select(p => new
            {
                p.ConnectionId,
                p.UserId,
                p.Username,
                ProfileImageUrl = profileImages.GetValueOrDefault(p.UserId),
                p.IsMuted,
                p.IsSpeaking,
                p.IsDeafened
            }).ToList());

        await Clients.OthersInGroup(VoiceGroup(channelId))
            .SendAsync("PeerJoined", new
            {
                ConnectionId = Context.ConnectionId,
                UserId = GetUserId(),
                Username = GetUsername(),
                ProfileImageUrl = profileImages.GetValueOrDefault(GetUserId()),
                IsMuted = false,
                IsSpeaking = false,
                IsDeafened = false
            });

        await NotifyVoiceRosterAsync(channelId);
    }

    public async Task LeaveVoiceChannel(Guid channelId)
    {
        rooms.Leave(channelId, Context.ConnectionId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, VoiceGroup(channelId));

        await Clients.OthersInGroup(VoiceGroup(channelId))
            .SendAsync("PeerLeft", Context.ConnectionId);

        await NotifyVoiceRosterAsync(channelId);
    }

    public async Task UpdateVoiceState(bool isSpeaking, bool isMuted, bool isDeafened)
    {
        var channelIds = rooms.GetChannelsForConnection(Context.ConnectionId);
        if (channelIds.Count == 0)
        {
            return;
        }

        rooms.UpdateVoiceState(Context.ConnectionId, isSpeaking, isMuted, isDeafened);

        var payload = new
        {
            ConnectionId = Context.ConnectionId,
            UserId = GetUserId(),
            IsSpeaking = isSpeaking,
            IsMuted = isMuted,
            IsDeafened = isDeafened
        };

        foreach (var channelId in channelIds)
        {
            await Clients.Group(VoiceGroup(channelId)).SendAsync("PeerState", payload);
            await NotifyVoiceStateAsync(channelId, isSpeaking, isMuted, isDeafened);
        }
    }

    // signalType: "offer" | "answer" | "ice-candidate"
    public async Task SendSignal(string targetConnectionId, string signalType, string payload)
    {
        if (!rooms.AreInSameRoom(Context.ConnectionId, targetConnectionId))
        {
            throw new HubException("Target peer is not in the same voice channel.");
        }

        await Clients.Client(targetConnectionId).SendAsync("ReceiveSignal", new
        {
            FromConnectionId = Context.ConnectionId,
            FromUserId = GetUserId(),
            SignalType = signalType,
            Payload = payload
        });
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var channelIds = rooms.GetChannelsForConnection(Context.ConnectionId);
        rooms.RemoveConnection(Context.ConnectionId);

        foreach (var channelId in channelIds)
        {
            await Clients.OthersInGroup(VoiceGroup(channelId))
                .SendAsync("PeerLeft", Context.ConnectionId);
            await NotifyVoiceRosterAsync(channelId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private async Task NotifyVoiceRosterAsync(Guid channelId)
    {
        var serverId = await ResolveServerIdAsync(channelId);
        if (serverId == Guid.Empty)
        {
            return;
        }

        await chatHub.Clients.Group(ChatHub.ServerGroup(serverId))
            .SendAsync("VoiceRosterUpdated", serverId, channelId);
    }

    private async Task NotifyVoiceStateAsync(Guid channelId, bool isSpeaking, bool isMuted, bool isDeafened)
    {
        var serverId = await ResolveServerIdAsync(channelId);
        if (serverId == Guid.Empty)
        {
            return;
        }

        await chatHub.Clients.Group(ChatHub.ServerGroup(serverId))
            .SendAsync("VoiceUserState", serverId, channelId, GetUserId(), isSpeaking, isMuted, isDeafened);
    }

    private async Task<Guid> ResolveServerIdAsync(Guid channelId)
    {
        return await db.Channels
            .AsNoTracking()
            .Where(c => c.Id == channelId)
            .Select(c => c.ServerId)
            .FirstOrDefaultAsync();
    }

    private Guid GetUserId()
    {
        var userIdClaim = Context.User?.FindFirstValue("sub");
        return string.IsNullOrEmpty(userIdClaim) ? Guid.Empty : Guid.Parse(userIdClaim);
    }

    private string GetUsername()
    {
        var givenName = Context.User?.FindFirstValue("given_name") ?? string.Empty;
        var familyName = Context.User?.FindFirstValue("family_name") ?? string.Empty;
        var username = $"{givenName} {familyName}".Trim();
        return string.IsNullOrEmpty(username) ? "Unknown" : username;
    }

    private static string VoiceGroup(Guid channelId) => $"voice-{channelId}";
}
