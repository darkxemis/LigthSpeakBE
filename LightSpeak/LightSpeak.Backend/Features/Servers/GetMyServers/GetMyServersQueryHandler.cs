namespace LightSpeak.Backend.Features.Servers.GetMyServers;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Servers.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetMyServersQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser
) : IRequestHandler<GetMyServersQuery, Result<IReadOnlyList<ServerResult>>>
{
    public async Task<Result<IReadOnlyList<ServerResult>>> Handle(
        GetMyServersQuery request,
        CancellationToken cancellationToken)
    {
        var servers = await db.ServerMembers
            .AsNoTracking()
            .Where(sm => sm.UserId == currentUser.UserId)
            .Select(sm => sm.Server)
            .Select(s => new ServerResult(
                s.Id,
                s.Name,
                s.IconUrl,
                s.OwnerId,
                s.InviteCode,
                s.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<ServerResult>>.Success(servers);
    }
}
