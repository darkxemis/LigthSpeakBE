namespace LightSpeak.Backend.Features.Servers.RegenerateInvite;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Servers.DTOs;
using MediatR;

public sealed record RegenerateInviteCommand(
    Guid ServerId) : IRequest<Result<ServerResult>>;
