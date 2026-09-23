namespace LightSpeak.Backend.Infrastructure.Services;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Dominio;
using Microsoft.EntityFrameworkCore;

public sealed class ServerMembershipService(IApplicationDbContext db) : IServerMembershipService
{
    public Task<bool> IsMemberAsync(
        Guid serverId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return db.ServerMembers.AnyAsync(
            sm => sm.ServerId == serverId && sm.UserId == userId,
            cancellationToken);
    }

    public async Task<ServerRole?> GetRoleAsync(
        Guid serverId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var role = await db.ServerMembers
            .Where(sm => sm.ServerId == serverId && sm.UserId == userId)
            .Select(sm => (ServerRole?)sm.Role)
            .FirstOrDefaultAsync(cancellationToken);

        return role;
    }

    public async Task<bool> HasRoleAtLeastAsync(
        Guid serverId,
        Guid userId,
        ServerRole minimumRole,
        CancellationToken cancellationToken = default)
    {
        var role = await GetRoleAsync(serverId, userId, cancellationToken);
        return role is { } current && current >= minimumRole;
    }

    public Task<bool> IsChannelMemberAsync(
        Guid channelId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return (
            from channel in db.Channels
            join member in db.ServerMembers on channel.ServerId equals member.ServerId
            where channel.Id == channelId && member.UserId == userId
            select member.UserId
        ).AnyAsync(cancellationToken);
    }
}
