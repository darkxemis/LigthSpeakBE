namespace LightSpeak.Backend.Features.Channels.DeleteChannel;

using LightSpeak.Backend.Common.Results;
using MediatR;

public sealed record DeleteChannelCommand(
    Guid ServerId,
    Guid ChannelId) : IRequest<Result>;
