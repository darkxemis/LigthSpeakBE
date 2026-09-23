namespace LightSpeak.Backend.Features.Messages.EditMessage;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Messages.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class EditMessageCommandHandler(
    IApplicationDbContext db,
    IServerMembershipService membership,
    ICurrentUserService currentUser
) : IRequestHandler<EditMessageCommand, Result<MessageResult>>
{
    public async Task<Result<MessageResult>> Handle(
        EditMessageCommand request,
        CancellationToken cancellationToken)
    {
        if (!await membership.IsChannelMemberAsync(request.ChannelId, currentUser.UserId, cancellationToken))
        {
            return Result<MessageResult>.Failure(Error.ChannelNotFound());
        }

        var message = await db.Messages
            .FirstOrDefaultAsync(
                m => m.Id == request.MessageId && m.ChannelId == request.ChannelId,
                cancellationToken);

        if (message is null)
        {
            return Result<MessageResult>.Failure(Error.MessageNotFound());
        }

        if (message.AuthorId != currentUser.UserId)
        {
            return Result<MessageResult>.Failure(Error.MessageNotAuthor());
        }

        message.Edit(request.Content.Trim());
        await db.SaveChangesAsync(cancellationToken);

        var author = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == message.AuthorId, cancellationToken);

        return Result<MessageResult>.Success(new MessageResult(
            message.Id,
            message.ChannelId,
            message.AuthorId,
            author?.FirstName ?? string.Empty,
            author?.LastName ?? string.Empty,
            author?.ProfileImageUrl,
            message.Content,
            message.CreatedAt,
            message.EditedAt));
    }
}
