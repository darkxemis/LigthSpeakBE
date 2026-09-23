namespace LightSpeak.Backend.Features.Messages.DeleteMessage;

using LightSpeak.Backend.Common.Results;
using MediatR;

public sealed record DeleteMessageCommand(
    Guid ChannelId,
    Guid MessageId) : IRequest<Result>;
