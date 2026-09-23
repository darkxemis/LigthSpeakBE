namespace LightSpeak.Backend.Features.Channels.GetChannels;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Channels.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetChannelsQueryHandler(
    IApplicationDbContext db,
    IServerMembershipService membership,
    ICurrentUserService currentUser
) : IRequestHandler<GetChannelsQuery, Result<IReadOnlyList<ChannelResult>>>
{
    public async Task<Result<IReadOnlyList<ChannelResult>>> Handle(
        GetChannelsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await membership.IsMemberAsync(request.ServerId, currentUser.UserId, cancellationToken))
        {
            return Result<IReadOnlyList<ChannelResult>>.Failure(Error.ServerNotMember());
        }

        var channels = await db.Channels
            .AsNoTracking()
            .Where(c => c.ServerId == request.ServerId)
            .OrderBy(c => c.Position)
            .ThenBy(c => c.CreatedAt)
            .Select(c => new ChannelResult(
                c.Id,
                c.ServerId,
                c.Name,
                c.Type,
                c.Position,
                c.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<ChannelResult>>.Success(channels);
    }
}
