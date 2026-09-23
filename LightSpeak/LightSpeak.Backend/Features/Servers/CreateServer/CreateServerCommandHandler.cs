namespace LightSpeak.Backend.Features.Servers.CreateServer;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using LightSpeak.Backend.Features.Servers.DTOs;
using MediatR;

public sealed class CreateServerCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser
) : IRequestHandler<CreateServerCommand, Result<ServerResult>>
{
    public async Task<Result<ServerResult>> Handle(
        CreateServerCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var server = Server.Create(request.Name, userId);
        server.Members.Add(ServerMember.Create(server.Id, userId, ServerRole.Owner));
        server.Channels.Add(Channel.Create(server.Id, "general", ChannelType.Text, position: 0));
        server.Channels.Add(Channel.Create(server.Id, "General", ChannelType.Voice, position: 1));

        await db.Servers.AddAsync(server, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return Result<ServerResult>.Success(ToResult(server));
    }

    private static ServerResult ToResult(Server server) =>
        new(server.Id, server.Name, server.IconUrl, server.OwnerId, server.InviteCode, server.CreatedAt);
}
