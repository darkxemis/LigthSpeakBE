namespace LightSpeak.Backend.Features.Servers.CreateServer;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Servers.DTOs;
using MediatR;

public sealed record CreateServerCommand(
    string Name) : IRequest<Result<ServerResult>>;
