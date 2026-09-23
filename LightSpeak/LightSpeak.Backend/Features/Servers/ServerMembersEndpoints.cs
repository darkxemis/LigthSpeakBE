namespace LightSpeak.Backend.Features.Servers;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Servers.DTOs;
using LightSpeak.Backend.Features.Servers.GetMembers;
using LightSpeak.Backend.Features.Servers.KickMember;
using LightSpeak.Backend.Features.Servers.UpdateMemberRole;
using LightSpeak.Backend.Infrastructure.Middleware;
using LightSpeak.Backend.Dominio;
using MediatR;

public static class ServerMembersEndpoints
{
    public static IEndpointRouteBuilder MapServerMembersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/servers/{serverId:guid}/members").WithTags("Server Members");

        group.MapGet("/",
            async (
                Guid serverId,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetServerMembersQuery(serverId), cancellationToken);
                return result.ToHttpResult(http, Results.Ok);
            })
            .RequireAuthorization()
            .Produces<IReadOnlyList<ServerMemberResult>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("GetServerMembers")
            .WithSummary("GET /api/v1/servers/{serverId}/members")
            .WithDescription("Returns the members of a server the current user belongs to.");

        group.MapDelete("/{userId:guid}",
            async (
                Guid serverId,
                Guid userId,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new KickMemberCommand(serverId, userId), cancellationToken);
                return result.ToHttpResult(http, Results.NoContent);
            })
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("KickServerMember")
            .WithSummary("DELETE /api/v1/servers/{serverId}/members/{userId}")
            .WithDescription("Removes a member from the server. Admins can kick Members; the owner can kick anyone except themselves.");

        group.MapPut("/{userId:guid}/role",
            async (
                Guid serverId,
                Guid userId,
                UpdateMemberRoleCommand command,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    command with { ServerId = serverId, TargetUserId = userId },
                    cancellationToken);
                return result.ToHttpResult(http, Results.NoContent);
            })
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("UpdateServerMemberRole")
            .WithSummary("PUT /api/v1/servers/{serverId}/members/{userId}/role")
            .WithDescription("Changes a member's role to Admin or Member. Owner only.");

        return endpoints;
    }
}
