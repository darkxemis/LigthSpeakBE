namespace LightSpeak.Backend.Features.Messages.GetChannelMessages;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Messages.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetChannelMessagesQueryHandler(
    IApplicationDbContext db,
    IServerMembershipService membership,
    ICurrentUserService currentUser
) : IRequestHandler<GetChannelMessagesQuery, Result<MessagePageResult>>
{
    public async Task<Result<MessagePageResult>> Handle(
        GetChannelMessagesQuery request,
        CancellationToken cancellationToken)
    {
        if (!await membership.IsChannelMemberAsync(request.ChannelId, currentUser.UserId, cancellationToken))
        {
            return Result<MessagePageResult>.Failure(Error.ChannelNotFound());
        }

        var query = db.Messages
            .AsNoTracking()
            .Where(m => m.ChannelId == request.ChannelId);

        if (request.BeforeId is { } beforeId)
        {
            var cursorCreatedAt = await db.Messages
                .AsNoTracking()
                .Where(m => m.Id == beforeId && m.ChannelId == request.ChannelId)
                .Select(m => (DateTime?)m.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (cursorCreatedAt is null)
            {
                return Result<MessagePageResult>.Success(new MessagePageResult([], false));
            }

            query = query.Where(m => m.CreatedAt < cursorCreatedAt.Value);
        }

        var fetched = await query
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Take(request.Take + 1)
            .Select(m => new MessageResult(
                m.Id,
                m.ChannelId,
                m.AuthorId,
                m.Author.FirstName,
                m.Author.LastName,
                m.Author.ProfileImageUrl,
                m.Content,
                m.CreatedAt,
                m.EditedAt))
            .ToListAsync(cancellationToken);

        var hasMore = fetched.Count > request.Take;

        if (hasMore)
        {
            fetched.RemoveAt(fetched.Count - 1);
        }

        fetched.Reverse();

        return Result<MessagePageResult>.Success(new MessagePageResult(fetched, hasMore));
    }
}
