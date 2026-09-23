namespace LightSpeak.Backend.Features.Servers.DeleteServerIcon;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Servers.DTOs;
using MediatR;

public sealed record DeleteServerIconCommand(
    Guid ServerId) : IRequest<Result<ServerResult>>;
