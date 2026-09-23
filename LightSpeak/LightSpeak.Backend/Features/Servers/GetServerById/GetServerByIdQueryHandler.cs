namespace LightSpeak.Backend.Features.Servers.GetServerById;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Servers.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetServerByIdQueryHandler(
    IApplicationDbContext db,
    IServerMembershipService membership,
    ICurrentUserService currentUser
) : IRequestHandler<GetServerByIdQuery, Result<ServerResult>>
{
    public async Task<Result<ServerResult>> Handle(
        GetServerByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!await membership.IsMemberAsync(request.ServerId, currentUser.UserId, cancellationToken))
        {
            return Result<ServerResult>.Failure(Error.ServerNotMember());
        }

        var server = await db.Servers
            .AsNoTracking()
            .Where(s => s.Id == request.ServerId)
            .Select(s => new ServerResult(
                s.Id,
                s.Name,
                s.IconUrl,
                s.OwnerId,
                s.InviteCode,
                s.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return server is null
            ? Result<ServerResult>.Failure(Error.ServerNotFound())
            : Result<ServerResult>.Success(server);
    }
}
