namespace LightSpeak.Backend.Features.Servers.JoinServer;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using LightSpeak.Backend.Features.Servers.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class JoinServerCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser
) : IRequestHandler<JoinServerCommand, Result<ServerResult>>
{
    public async Task<Result<ServerResult>> Handle(JoinServerCommand request, CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToLowerInvariant();

        var server = await db.Servers
            .FirstOrDefaultAsync(s => s.InviteCode == code, cancellationToken);

        if (server is null)
        {
            return Result<ServerResult>.Failure(Error.ServerInviteInvalid());
        }

        var alreadyMember = await db.ServerMembers
            .AnyAsync(sm => sm.ServerId == server.Id && sm.UserId == currentUser.UserId, cancellationToken);

        if (alreadyMember)
        {
            return Result<ServerResult>.Success(ToResult(server));
        }

        db.ServerMembers.Add(ServerMember.Create(server.Id, currentUser.UserId, ServerRole.Member));
        await db.SaveChangesAsync(cancellationToken);

        return Result<ServerResult>.Success(ToResult(server));
    }

    private static ServerResult ToResult(Server server) =>
        new(server.Id, server.Name, server.IconUrl, server.OwnerId, server.InviteCode, server.CreatedAt);
}
