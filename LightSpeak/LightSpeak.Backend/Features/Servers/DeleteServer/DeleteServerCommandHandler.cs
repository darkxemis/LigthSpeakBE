namespace LightSpeak.Backend.Features.Servers.DeleteServer;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteServerCommandHandler(
    IApplicationDbContext db,
    IServerMembershipService membership,
    ICurrentUserService currentUser,
    IFileStorageService fileStorage
) : IRequestHandler<DeleteServerCommand, Result>
{
    public async Task<Result> Handle(DeleteServerCommand request, CancellationToken cancellationToken)
    {
        var role = await membership.GetRoleAsync(request.ServerId, currentUser.UserId, cancellationToken);

        if (role is null)
        {
            return Result.Failure(Error.ServerNotMember());
        }

        if (role != ServerRole.Owner)
        {
            return Result.Failure(Error.ServerForbidden(nameof(ServerRole.Owner)));
        }

        var server = await db.Servers
            .FirstOrDefaultAsync(s => s.Id == request.ServerId, cancellationToken);

        if (server is null)
        {
            return Result.Failure(Error.ServerNotFound());
        }

        if (!string.IsNullOrWhiteSpace(server.IconUrl))
        {
            await fileStorage.DeleteServerIconAsync(server.IconUrl, cancellationToken);
        }

        db.Servers.Remove(server);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
