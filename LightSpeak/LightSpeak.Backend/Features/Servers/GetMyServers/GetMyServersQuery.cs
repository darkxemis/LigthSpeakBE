namespace LightSpeak.Backend.Features.Servers.GetMyServers;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Servers.DTOs;
using MediatR;

public sealed record GetMyServersQuery : IRequest<Result<IReadOnlyList<ServerResult>>>;
