namespace LightSpeak.Backend.Features.Servers.GetServerById;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Servers.DTOs;
using MediatR;

public sealed record GetServerByIdQuery(
    Guid ServerId) : IRequest<Result<ServerResult>>;
