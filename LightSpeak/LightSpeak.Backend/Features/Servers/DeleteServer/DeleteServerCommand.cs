namespace LightSpeak.Backend.Features.Servers.DeleteServer;

using LightSpeak.Backend.Common.Results;
using MediatR;

public sealed record DeleteServerCommand(
    Guid ServerId) : IRequest<Result>;
