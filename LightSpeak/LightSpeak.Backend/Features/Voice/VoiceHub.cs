namespace LightSpeak.Backend.Features.Voice;

using System.Security.Claims;
using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

// Signaling only: relays WebRTC offers/answers/ICE candidates between peers in
// the same voice channel. Audio/video/screen-share travels P2P between clients.
[Authorize]
public sealed class VoiceHub(
    IServerMembershipService membership,
    IVoiceRoomRegistry rooms
) : Hub
{
    public async Task JoinVoiceChannel(Guid channelId)
    {
        if (!await membership.IsChannelMemberAsync(channelId, GetUserId()))
        {
            throw new HubException(Error.ChannelNotFound().Message);
        }

        var existingPeers = rooms.Join(channelId, Context.ConnectionId, GetUserId(), GetUsername());

        await Groups.AddToGroupAsync(Context.ConnectionId, VoiceGroup(channelId));

        await Clients.Caller.SendAsync("ExistingPeers", existingPeers);

        await Clients.OthersInGroup(VoiceGroup(channelId))
            .SendAsync("PeerJoined", new
            {
                ConnectionId = Context.ConnectionId,
                UserId = GetUserId(),
                Username = GetUsername()
            });
    }

    public async Task LeaveVoiceChannel(Guid channelId)
    {
        rooms.Leave(channelId, Context.ConnectionId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, VoiceGroup(channelId));

        await Clients.OthersInGroup(VoiceGroup(channelId))
            .SendAsync("PeerLeft", Context.ConnectionId);
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

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        rooms.RemoveConnection(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
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
