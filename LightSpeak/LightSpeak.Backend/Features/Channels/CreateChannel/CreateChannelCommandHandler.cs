namespace LightSpeak.Backend.Features.Channels.CreateChannel;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using LightSpeak.Backend.Features.Channels.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class CreateChannelCommandHandler(
    IApplicationDbContext db,
    IServerMembershipService membership,
    ICurrentUserService currentUser
) : IRequestHandler<CreateChannelCommand, Result<ChannelResult>>
{
    public async Task<Result<ChannelResult>> Handle(
        CreateChannelCommand request,
        CancellationToken cancellationToken)
    {
        if (!await membership.HasRoleAtLeastAsync(
                request.ServerId, currentUser.UserId, ServerRole.Admin, cancellationToken))
        {
            return Result<ChannelResult>.Failure(
                await membership.IsMemberAsync(request.ServerId, currentUser.UserId, cancellationToken)
                    ? Error.ServerForbidden(nameof(ServerRole.Admin))
                    : Error.ServerNotMember());
        }

        var serverExists = await db.Servers
            .AnyAsync(s => s.Id == request.ServerId, cancellationToken);

        if (!serverExists)
        {
            return Result<ChannelResult>.Failure(Error.ServerNotFound());
        }

        var lastPosition = await db.Channels
            .Where(c => c.ServerId == request.ServerId)
            .MaxAsync(c => (int?)c.Position, cancellationToken) ?? -1;

        var channel = Channel.Create(
            request.ServerId,
            request.Name.Trim(),
            request.Type,
            lastPosition + 1);

        await db.Channels.AddAsync(channel, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return Result<ChannelResult>.Success(ToResult(channel));
    }

    private static ChannelResult ToResult(Channel channel) =>
        new(channel.Id, channel.ServerId, channel.Name, channel.Type, channel.Position, channel.CreatedAt);
}
