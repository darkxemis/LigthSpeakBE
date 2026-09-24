namespace LightSpeak.Backend.Features.Chat;

using System.Security.Claims;
using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Messages.SendMessage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

[Authorize]
public sealed class ChatHub(
    ISender sender,
    IServerMembershipService membership
) : Hub
{
    public async Task JoinChannel(Guid channelId)
    {
        if (!await membership.IsChannelMemberAsync(channelId, GetUserId()))
        {
            throw new HubException(Error.ChannelNotFound().Message);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, ChannelGroup(channelId));
    }

    public Task LeaveChannel(Guid channelId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, ChannelGroup(channelId));
    }

    public async Task JoinServerGroup(Guid serverId)
    {
        if (!await membership.IsMemberAsync(serverId, GetUserId()))
        {
            throw new HubException(Error.ServerNotMember().Message);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, ServerGroup(serverId));
    }

    public Task LeaveServerGroup(Guid serverId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, ServerGroup(serverId));
    }

    public async Task SendMessage(Guid channelId, string content)
    {
        var result = await sender.Send(new SendMessageCommand(channelId, content));

        if (result.IsFailure)
        {
            throw new HubException(result.Error!.Message);
        }

        await Clients.Group(ChannelGroup(channelId)).SendAsync("ReceiveMessage", result.Value);
    }

    public async Task Typing(Guid channelId)
    {
        if (!await membership.IsChannelMemberAsync(channelId, GetUserId()))
        {
            return;
        }

        await Clients.OthersInGroup(ChannelGroup(channelId))
            .SendAsync("UserTyping", GetUserId(), GetUsername());
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

    private static string ChannelGroup(Guid channelId) => $"channel-{channelId}";

    public static string ServerGroup(Guid serverId) => $"server-{serverId}";
}
