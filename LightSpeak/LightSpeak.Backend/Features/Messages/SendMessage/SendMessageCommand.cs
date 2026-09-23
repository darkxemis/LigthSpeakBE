namespace LightSpeak.Backend.Features.Messages.SendMessage;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Messages.DTOs;
using MediatR;

public sealed record SendMessageCommand(
    Guid ChannelId,
    string Content) : IRequest<Result<MessageResult>>;
