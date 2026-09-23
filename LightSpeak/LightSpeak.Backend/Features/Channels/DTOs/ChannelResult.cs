namespace LightSpeak.Backend.Features.Channels.DTOs;

using LightSpeak.Backend.Dominio;

public sealed record ChannelResult(
    Guid Id,
    Guid ServerId,
    string Name,
    ChannelType Type,
    int Position,
    DateTime CreatedAt);
