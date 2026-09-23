namespace LightSpeak.Backend.Features.Servers.GetMembers;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Servers.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetServerMembersQueryHandler(
    IApplicationDbContext db,
    IServerMembershipService membership,
    ICurrentUserService currentUser
) : IRequestHandler<GetServerMembersQuery, Result<IReadOnlyList<ServerMemberResult>>>
{
    public async Task<Result<IReadOnlyList<ServerMemberResult>>> Handle(
        GetServerMembersQuery request,
        CancellationToken cancellationToken)
    {
        if (!await membership.IsMemberAsync(request.ServerId, currentUser.UserId, cancellationToken))
        {
            return Result<IReadOnlyList<ServerMemberResult>>.Failure(Error.ServerNotMember());
        }

        var members = await db.ServerMembers
            .AsNoTracking()
            .Where(sm => sm.ServerId == request.ServerId)
            .Select(sm => new ServerMemberResult(
                sm.User.Id,
                sm.User.FirstName,
                sm.User.LastName,
                sm.User.ProfileImageUrl,
                sm.Role,
                sm.JoinedAt))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<ServerMemberResult>>.Success(members);
    }
}
