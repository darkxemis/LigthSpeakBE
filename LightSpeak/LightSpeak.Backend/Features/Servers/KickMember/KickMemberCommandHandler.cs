namespace LightSpeak.Backend.Features.Servers.KickMember;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class KickMemberCommandHandler(
    IApplicationDbContext db,
    IServerMembershipService membership,
    ICurrentUserService currentUser
) : IRequestHandler<KickMemberCommand, Result>
{
    public async Task<Result> Handle(KickMemberCommand request, CancellationToken cancellationToken)
    {
        var actorId = currentUser.UserId;

        if (request.TargetUserId == actorId)
        {
            return Result.Failure(Error.ServerMemberForbidden());
        }

        var actorRole = await membership.GetRoleAsync(request.ServerId, actorId, cancellationToken);

        if (actorRole is null)
        {
            return Result.Failure(Error.ServerNotMember());
        }

        if (actorRole == ServerRole.Member)
        {
            return Result.Failure(Error.ServerForbidden(nameof(ServerRole.Admin)));
        }

        var targetMember = await db.ServerMembers
            .FirstOrDefaultAsync(
                sm => sm.ServerId == request.ServerId && sm.UserId == request.TargetUserId,
                cancellationToken);

        if (targetMember is null)
        {
            return Result.Failure(Error.ServerNotMember());
        }

        if (targetMember.Role >= actorRole)
        {
            return Result.Failure(Error.ServerMemberForbidden());
        }

        db.ServerMembers.Remove(targetMember);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
