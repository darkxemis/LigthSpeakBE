namespace LightSpeak.Backend.Features.Messages.DeleteMessage;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteMessageCommandHandler(
    IApplicationDbContext db,
    IServerMembershipService membership,
    ICurrentUserService currentUser
) : IRequestHandler<DeleteMessageCommand, Result>
{
    public async Task<Result> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        if (!await membership.IsChannelMemberAsync(request.ChannelId, currentUser.UserId, cancellationToken))
        {
            return Result.Failure(Error.ChannelNotFound());
        }

        var message = await db.Messages
            .FirstOrDefaultAsync(
                m => m.Id == request.MessageId && m.ChannelId == request.ChannelId,
                cancellationToken);

        if (message is null)
        {
            return Result.Failure(Error.MessageNotFound());
        }

        if (message.AuthorId != currentUser.UserId)
        {
            var serverId = await db.Channels
                .Where(c => c.Id == request.ChannelId)
                .Select(c => c.ServerId)
                .FirstAsync(cancellationToken);

            var role = await membership.GetRoleAsync(serverId, currentUser.UserId, cancellationToken);

            if (role is not (ServerRole.Owner or ServerRole.Admin))
            {
                return Result.Failure(Error.MessageNotAuthor());
            }
        }

        db.Messages.Remove(message);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
