namespace LightSpeak.Backend.Features.Servers.KickMember;

using LightSpeak.Backend.Common.Results;
using MediatR;

public sealed record KickMemberCommand(
    Guid ServerId,
    Guid TargetUserId) : IRequest<Result>;
