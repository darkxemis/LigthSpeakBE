namespace LightSpeak.Backend.Features.Messages.SendMessage;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using LightSpeak.Backend.Features.Messages.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class SendMessageCommandHandler(
    IApplicationDbContext db,
    IServerMembershipService membership,
    ICurrentUserService currentUser
) : IRequestHandler<SendMessageCommand, Result<MessageResult>>
{
    private const int RateWindowSeconds = 10;
    private const int MaxMessagesPerWindow = 5;

    public async Task<Result<MessageResult>> Handle(
        SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        if (!await membership.IsChannelMemberAsync(request.ChannelId, currentUser.UserId, cancellationToken))
        {
            return Result<MessageResult>.Failure(Error.ChannelNotFound());
        }

        var rateLimitError = await CheckRateLimitAsync(request.ChannelId, cancellationToken);
        if (rateLimitError is not null)
        {
            return Result<MessageResult>.Failure(rateLimitError);
        }

        var message = Message.Create(
            request.ChannelId,
            currentUser.UserId,
            request.Content.Trim());

        await db.Messages.AddAsync(message, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var author = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUser.UserId, cancellationToken);

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

    private async Task<Error?> CheckRateLimitAsync(Guid channelId, CancellationToken cancellationToken)
    {
        var windowStart = DateTime.UtcNow.AddSeconds(-RateWindowSeconds);

        var recentCount = await db.Messages
            .CountAsync(
                m => m.AuthorId == currentUser.UserId &&
                     m.ChannelId == channelId &&
                     m.CreatedAt >= windowStart,
                cancellationToken);

        if (recentCount >= MaxMessagesPerWindow)
        {
            return Error.MessagesRateLimited(RateWindowSeconds);
        }

        return null;
    }
}
