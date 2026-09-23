namespace LightSpeak.Backend.Features.Servers.UpdateServer;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Servers.DTOs;
using MediatR;

public sealed record UpdateServerCommand(
    Guid ServerId,
    string Name) : IRequest<Result<ServerResult>>;
