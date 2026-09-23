namespace LightSpeak.Backend.Features.Servers.UpdateMemberRole;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class UpdateMemberRoleCommandHandler(
    IApplicationDbContext db,
    IServerMembershipService membership,
    ICurrentUserService currentUser
) : IRequestHandler<UpdateMemberRoleCommand, Result>
{
    public async Task<Result> Handle(UpdateMemberRoleCommand request, CancellationToken cancellationToken)
    {
        var actorRole = await membership.GetRoleAsync(
            request.ServerId, currentUser.UserId, cancellationToken);

        if (actorRole is null)
        {
            return Result.Failure(Error.ServerNotMember());
        }

        if (actorRole != ServerRole.Owner)
        {
            return Result.Failure(Error.ServerForbidden(nameof(ServerRole.Owner)));
        }

        if (request.TargetUserId == currentUser.UserId)
        {
            return Result.Failure(Error.ServerMemberForbidden());
        }

        var targetMember = await db.ServerMembers
            .FirstOrDefaultAsync(
                sm => sm.ServerId == request.ServerId && sm.UserId == request.TargetUserId,
                cancellationToken);

        if (targetMember is null)
        {
            return Result.Failure(Error.ServerNotMember());
        }

        if (targetMember.Role == ServerRole.Owner)
        {
            return Result.Failure(Error.ServerMemberForbidden());
        }

        targetMember.SetRole(request.Role);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
