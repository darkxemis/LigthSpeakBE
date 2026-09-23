namespace LightSpeak.Backend.Features.Messages.EditMessage;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Messages.DTOs;
using MediatR;

public sealed record EditMessageCommand(
    Guid ChannelId,
    Guid MessageId,
    string Content) : IRequest<Result<MessageResult>>;
