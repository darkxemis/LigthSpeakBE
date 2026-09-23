namespace LightSpeak.Backend.Features.Messages.DTOs;

public sealed record MessageResult(
    Guid Id,
    Guid ChannelId,
    Guid AuthorId,
    string AuthorFirstName,
    string AuthorLastName,
    string? AuthorProfileImageUrl,
    string Content,
    DateTime CreatedAt,
    DateTime? EditedAt);

public sealed record MessagePageResult(
    IReadOnlyList<MessageResult> Messages,
    bool HasMore);
