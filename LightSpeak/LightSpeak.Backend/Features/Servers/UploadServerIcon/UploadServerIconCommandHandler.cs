namespace LightSpeak.Backend.Features.Servers.UploadServerIcon;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using LightSpeak.Backend.Features.Servers.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class UploadServerIconCommandHandler(
    IApplicationDbContext db,
    IServerMembershipService membership,
    ICurrentUserService currentUser,
    IFileStorageService fileStorage
) : IRequestHandler<UploadServerIconCommand, Result<ServerResult>>
{
    public async Task<Result<ServerResult>> Handle(
        UploadServerIconCommand request,
        CancellationToken cancellationToken)
    {
        if (!await membership.HasRoleAtLeastAsync(
                request.ServerId, currentUser.UserId, ServerRole.Admin, cancellationToken))
        {
            return Result<ServerResult>.Failure(
                await membership.IsMemberAsync(request.ServerId, currentUser.UserId, cancellationToken)
                    ? Error.ServerForbidden(nameof(ServerRole.Admin))
                    : Error.ServerNotMember());
        }

        var server = await db.Servers
            .FirstOrDefaultAsync(s => s.Id == request.ServerId, cancellationToken);

        if (server is null)
        {
            return Result<ServerResult>.Failure(Error.ServerNotFound());
        }

        string iconUrl;

        try
        {
            iconUrl = await fileStorage.SaveServerIconAsync(
                server.Id, request.FileStream, request.FileName, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            return Result<ServerResult>.Failure(Error.InvalidImageFile());
        }

        if (!string.IsNullOrWhiteSpace(server.IconUrl))
        {
            await fileStorage.DeleteServerIconAsync(server.IconUrl, cancellationToken);
        }

        server.SetIcon(iconUrl);
        await db.SaveChangesAsync(cancellationToken);

        return Result<ServerResult>.Success(ToResult(server));
    }

    private static ServerResult ToResult(Server server) =>
        new(server.Id, server.Name, server.IconUrl, server.OwnerId, server.InviteCode, server.CreatedAt);
}
