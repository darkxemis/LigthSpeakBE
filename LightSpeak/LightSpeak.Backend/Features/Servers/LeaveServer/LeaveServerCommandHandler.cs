namespace LightSpeak.Backend.Features.Servers.LeaveServer;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class LeaveServerCommandHandler(
    IApplicationDbContext db,
    IServerMembershipService membership,
    ICurrentUserService currentUser
) : IRequestHandler<LeaveServerCommand, Result>
{
    public async Task<Result> Handle(LeaveServerCommand request, CancellationToken cancellationToken)
    {
        var role = await membership.GetRoleAsync(request.ServerId, currentUser.UserId, cancellationToken);

        if (role is null)
        {
            return Result.Failure(Error.ServerNotMember());
        }

        if (role == ServerRole.Owner)
        {
            return Result.Failure(Error.ServerOwnerCannotLeave());
        }

        var member = await db.ServerMembers
            .FirstOrDefaultAsync(
                sm => sm.ServerId == request.ServerId && sm.UserId == currentUser.UserId,
                cancellationToken);

        if (member is null)
        {
            return Result.Failure(Error.ServerNotMember());
        }

        db.ServerMembers.Remove(member);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
