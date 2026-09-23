namespace LightSpeak.Backend.Features.Servers.UpdateServer;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using LightSpeak.Backend.Features.Servers.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class UpdateServerCommandHandler(
    IApplicationDbContext db,
    IServerMembershipService membership,
    ICurrentUserService currentUser
) : IRequestHandler<UpdateServerCommand, Result<ServerResult>>
{
    public async Task<Result<ServerResult>> Handle(
        UpdateServerCommand request,
        CancellationToken cancellationToken)
    {
        if (!await membership.HasRoleAtLeastAsync(
                request.ServerId, currentUser.UserId, ServerRole.Admin, cancellationToken))
        {
            return Result<ServerResult>.Failure(await NotFoundOrForbidden(request.ServerId, cancellationToken));
        }

        var server = await db.Servers
            .FirstOrDefaultAsync(s => s.Id == request.ServerId, cancellationToken);

        if (server is null)
        {
            return Result<ServerResult>.Failure(Error.ServerNotFound());
        }

        server.Rename(request.Name);
        await db.SaveChangesAsync(cancellationToken);

        return Result<ServerResult>.Success(ToResult(server));
    }

    private async Task<Error> NotFoundOrForbidden(Guid serverId, CancellationToken cancellationToken)
    {
        return await membership.IsMemberAsync(serverId, currentUser.UserId, cancellationToken)
            ? Error.ServerForbidden(nameof(ServerRole.Admin))
            : Error.ServerNotMember();
    }

    private static ServerResult ToResult(Server server) =>
        new(server.Id, server.Name, server.IconUrl, server.OwnerId, server.InviteCode, server.CreatedAt);
}
