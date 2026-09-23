namespace LightSpeak.Backend.Features.Servers.LeaveServer;

using LightSpeak.Backend.Common.Results;
using MediatR;

public sealed record LeaveServerCommand(
    Guid ServerId) : IRequest<Result>;
