namespace LightSpeak.Backend.Features.Channels.GetChannels;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Channels.DTOs;
using MediatR;

public sealed record GetChannelsQuery(
    Guid ServerId) : IRequest<Result<IReadOnlyList<ChannelResult>>>;
