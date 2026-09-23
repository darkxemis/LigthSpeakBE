namespace LightSpeak.Backend.Features.Messages.GetChannelMessages;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Messages.DTOs;
using MediatR;

public sealed record GetChannelMessagesQuery(
    Guid ChannelId,
    Guid? BeforeId,
    int Take) : IRequest<Result<MessagePageResult>>;
