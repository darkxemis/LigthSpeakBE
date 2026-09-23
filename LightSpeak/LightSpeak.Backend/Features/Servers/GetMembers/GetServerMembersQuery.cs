namespace LightSpeak.Backend.Features.Servers.GetMembers;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Servers.DTOs;
using MediatR;

public sealed record GetServerMembersQuery(
    Guid ServerId) : IRequest<Result<IReadOnlyList<ServerMemberResult>>>;
