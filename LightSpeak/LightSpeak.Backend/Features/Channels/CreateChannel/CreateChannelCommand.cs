namespace LightSpeak.Backend.Features.Channels.CreateChannel;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using LightSpeak.Backend.Features.Channels.DTOs;
using MediatR;

public sealed record CreateChannelCommand(
    Guid ServerId,
    string Name,
    ChannelType Type) : IRequest<Result<ChannelResult>>;
