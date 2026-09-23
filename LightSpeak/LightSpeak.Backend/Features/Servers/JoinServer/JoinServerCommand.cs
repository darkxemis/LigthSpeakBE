namespace LightSpeak.Backend.Features.Servers.JoinServer;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Servers.DTOs;
using MediatR;

public sealed record JoinServerCommand(
    string Code) : IRequest<Result<ServerResult>>;
