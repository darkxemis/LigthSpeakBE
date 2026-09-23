namespace LightSpeak.Backend.Features.Channels.DeleteChannel;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteChannelCommandHandler(
    IApplicationDbContext db,
    IServerMembershipService membership,
    ICurrentUserService currentUser
) : IRequestHandler<DeleteChannelCommand, Result>
{
    public async Task<Result> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
    {
        if (!await membership.HasRoleAtLeastAsync(
                request.ServerId, currentUser.UserId, ServerRole.Admin, cancellationToken))
        {
            return Result.Failure(
                await membership.IsMemberAsync(request.ServerId, currentUser.UserId, cancellationToken)
                    ? Error.ServerForbidden(nameof(ServerRole.Admin))
                    : Error.ServerNotMember());
        }

        var channel = await db.Channels
            .FirstOrDefaultAsync(
                c => c.Id == request.ChannelId && c.ServerId == request.ServerId,
                cancellationToken);

        if (channel is null)
        {
            return Result.Failure(Error.ChannelNotFound());
        }

        db.Channels.Remove(channel);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
