namespace LightSpeak.Backend.Features.Voice;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using LightSpeak.Backend.Infrastructure.Middleware;
using Microsoft.EntityFrameworkCore;

public static class VoiceEndpoints
{
    public static IEndpointRouteBuilder MapVoiceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/servers/{serverId:guid}/voice").WithTags("Voice");

        group.MapGet(
                "/participants",
                async (
                    Guid serverId,
                    IServerMembershipService membership,
                    IVoiceRoomRegistry rooms,
                    IApplicationDbContext db,
                    ICurrentUserService currentUser,
                    HttpContext http,
                    CancellationToken cancellationToken) =>
                {
                    if (!await membership.IsMemberAsync(serverId, currentUser.UserId, cancellationToken))
                    {
                        return Result<IReadOnlyList<VoiceChannelParticipants>>
                            .Failure(Error.ServerNotMember())
                            .ToHttpResult(http, Results.Ok);
                    }

                    var voiceChannelIds = await db.Channels
                        .AsNoTracking()
                        .Where(c => c.ServerId == serverId && c.Type == ChannelType.Voice)
                        .Select(c => c.Id)
                        .ToListAsync(cancellationToken);

                    var channelPeers = voiceChannelIds
                        .Select(channelId => (ChannelId: channelId, Peers: rooms.GetChannelPeers(channelId).ToList()))
                        .ToList();

                    var userIds = channelPeers
                        .SelectMany(c => c.Peers)
                        .Select(p => p.UserId)
                        .Distinct()
                        .ToList();

                    var profileImages = new Dictionary<Guid, string?>();
                    if (userIds.Count > 0)
                    {
                        profileImages = await db.Users
                            .AsNoTracking()
                            .Where(u => userIds.Contains(u.Id))
                            .ToDictionaryAsync(u => u.Id, u => u.ProfileImageUrl, cancellationToken);
                    }

                    var result = channelPeers
                        .Select(c => new VoiceChannelParticipants(
                            c.ChannelId,
                            c.Peers
                                .Select(p => new VoiceParticipant(
                                    p.UserId,
                                    p.Username,
                                    profileImages.GetValueOrDefault(p.UserId),
                                    p.IsMuted,
                                    p.IsSpeaking,
                                    p.IsDeafened))
                                .ToList()))
                        .ToList();

                    return Result<IReadOnlyList<VoiceChannelParticipants>>
                        .Success(result)
                        .ToHttpResult(http, Results.Ok);
                })
            .RequireAuthorization()
            .Produces<IReadOnlyList<VoiceChannelParticipants>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("GetVoiceParticipants")
            .WithSummary("GET /api/v1/servers/{serverId}/voice/participants")
            .WithDescription("Returns who is currently connected to each voice channel of a server.");

        return endpoints;
    }
}

public sealed record VoiceParticipant(
    Guid UserId,
    string Username,
    string? ProfileImageUrl,
    bool IsMuted,
    bool IsSpeaking,
    bool IsDeafened);

public sealed record VoiceChannelParticipants(Guid ChannelId, IReadOnlyList<VoiceParticipant> Participants);
