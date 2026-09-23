namespace LightSpeak.Backend.Common.Interfaces;

using LightSpeak.Backend.Dominio;

public interface IServerMembershipService
{
    Task<bool> IsMemberAsync(Guid serverId, Guid userId, CancellationToken cancellationToken = default);

    Task<ServerRole?> GetRoleAsync(Guid serverId, Guid userId, CancellationToken cancellationToken = default);

    Task<bool> HasRoleAtLeastAsync(
        Guid serverId,
        Guid userId,
        ServerRole minimumRole,
        CancellationToken cancellationToken = default);

    Task<bool> IsChannelMemberAsync(Guid channelId, Guid userId, CancellationToken cancellationToken = default);
}
