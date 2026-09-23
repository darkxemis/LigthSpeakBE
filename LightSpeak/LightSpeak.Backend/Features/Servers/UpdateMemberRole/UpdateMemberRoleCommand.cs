namespace LightSpeak.Backend.Features.Servers.UpdateMemberRole;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using MediatR;

public sealed record UpdateMemberRoleCommand(
    Guid ServerId,
    Guid TargetUserId,
    ServerRole Role) : IRequest<Result>;
