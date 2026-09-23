namespace LightSpeak.Backend.Features.Servers;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Servers.CreateServer;
using LightSpeak.Backend.Features.Servers.DeleteServer;
using LightSpeak.Backend.Features.Servers.DeleteServerIcon;
using LightSpeak.Backend.Features.Servers.DTOs;
using LightSpeak.Backend.Features.Servers.GetMyServers;
using LightSpeak.Backend.Features.Servers.GetServerById;
using LightSpeak.Backend.Features.Servers.JoinServer;
using LightSpeak.Backend.Features.Servers.LeaveServer;
using LightSpeak.Backend.Features.Servers.RegenerateInvite;
using LightSpeak.Backend.Features.Servers.UpdateServer;
using LightSpeak.Backend.Features.Servers.UploadServerIcon;
using LightSpeak.Backend.Infrastructure.Middleware;
using MediatR;

public static class ServersEndpoints
{
    public static IEndpointRouteBuilder MapServersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/servers").WithTags("Servers");

        group.MapGet("/",
            async (ISender sender, HttpContext http, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetMyServersQuery(), cancellationToken);
                return result.ToHttpResult(http, Results.Ok);
            })
            .RequireAuthorization()
            .Produces<IReadOnlyList<ServerResult>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .WithName("GetMyServers")
            .WithSummary("GET /api/v1/servers")
            .WithDescription("Returns the servers the current user belongs to.");

        group.MapPost("/",
            async (
                CreateServerCommand command,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return result.ToHttpResult(http,
                    server => Results.Created($"/api/v1/servers/{server.Id}", server));
            })
            .RequireAuthorization()
            .Produces<ServerResult>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .WithName("CreateServer")
            .WithSummary("POST /api/v1/servers")
            .WithDescription("Creates a server with default text and voice channels. The creator becomes owner.");

        group.MapPost("/join",
            async (
                JoinServerCommand command,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return result.ToHttpResult(http, Results.Ok);
            })
            .RequireAuthorization()
            .Produces<ServerResult>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("JoinServer")
            .WithSummary("POST /api/v1/servers/join")
            .WithDescription("Joins a server using an invite code. Idempotent if already a member.");

        group.MapGet("/{serverId:guid}",
            async (
                Guid serverId,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetServerByIdQuery(serverId), cancellationToken);
                return result.ToHttpResult(http, Results.Ok);
            })
            .RequireAuthorization()
            .Produces<ServerResult>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("GetServerById")
            .WithSummary("GET /api/v1/servers/{serverId}")
            .WithDescription("Returns a server the current user is a member of.");

        group.MapPatch("/{serverId:guid}",
            async (
                Guid serverId,
                UpdateServerCommand command,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command with { ServerId = serverId }, cancellationToken);
                return result.ToHttpResult(http, Results.Ok);
            })
            .RequireAuthorization()
            .Produces<ServerResult>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("UpdateServer")
            .WithSummary("PATCH /api/v1/servers/{serverId}")
            .WithDescription("Renames a server. Requires Admin or Owner role.");

        group.MapDelete("/{serverId:guid}",
            async (
                Guid serverId,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteServerCommand(serverId), cancellationToken);
                return result.ToHttpResult(http, Results.NoContent);
            })
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("DeleteServer")
            .WithSummary("DELETE /api/v1/servers/{serverId}")
            .WithDescription("Deletes a server and all its channels and messages. Owner only.");

        group.MapPost("/{serverId:guid}/leave",
            async (
                Guid serverId,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new LeaveServerCommand(serverId), cancellationToken);
                return result.ToHttpResult(http, Results.NoContent);
            })
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("LeaveServer")
            .WithSummary("POST /api/v1/servers/{serverId}/leave")
            .WithDescription("Removes the current user from a server. The owner must delete instead.");

        group.MapPut("/{serverId:guid}/icon",
            async (
                Guid serverId,
                IFormFile file,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                await using var stream = file.OpenReadStream();
                var result = await sender.Send(
                    new UploadServerIconCommand(serverId, stream, file.FileName),
                    cancellationToken);
                return result.ToHttpResult(http, Results.Ok);
            })
            .RequireAuthorization()
            .DisableAntiforgery()
            .Produces<ServerResult>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("UploadServerIcon")
            .WithSummary("PUT /api/v1/servers/{serverId}/icon")
            .WithDescription("Uploads a server icon image. Replaces any existing icon. Requires Admin or Owner.");

        group.MapDelete("/{serverId:guid}/icon",
            async (
                Guid serverId,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteServerIconCommand(serverId), cancellationToken);
                return result.ToHttpResult(http, Results.Ok);
            })
            .RequireAuthorization()
            .Produces<ServerResult>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("DeleteServerIcon")
            .WithSummary("DELETE /api/v1/servers/{serverId}/icon")
            .WithDescription("Deletes the server icon. Requires Admin or Owner.");

        group.MapPost("/{serverId:guid}/invite",
            async (
                Guid serverId,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new RegenerateInviteCommand(serverId), cancellationToken);
                return result.ToHttpResult(http, Results.Ok);
            })
            .RequireAuthorization()
            .Produces<ServerResult>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("RegenerateInvite")
            .WithSummary("POST /api/v1/servers/{serverId}/invite")
            .WithDescription("Regenerates the invite code, invalidating the previous one. Requires Admin or Owner.");

        return endpoints;
    }
}
