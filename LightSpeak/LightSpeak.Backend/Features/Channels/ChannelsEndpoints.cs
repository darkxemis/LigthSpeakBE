namespace LightSpeak.Backend.Features.Channels;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Channels.CreateChannel;
using LightSpeak.Backend.Features.Channels.DeleteChannel;
using LightSpeak.Backend.Features.Channels.DTOs;
using LightSpeak.Backend.Features.Channels.GetChannels;
using LightSpeak.Backend.Infrastructure.Middleware;
using MediatR;

public static class ChannelsEndpoints
{
    public static IEndpointRouteBuilder MapChannelsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/servers/{serverId:guid}/channels").WithTags("Channels");

        group.MapGet("/",
            async (
                Guid serverId,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetChannelsQuery(serverId), cancellationToken);
                return result.ToHttpResult(http, Results.Ok);
            })
            .RequireAuthorization()
            .Produces<IReadOnlyList<ChannelResult>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("GetChannels")
            .WithSummary("GET /api/v1/servers/{serverId}/channels")
            .WithDescription("Returns the channels of a server the current user belongs to.");

        group.MapPost("/",
            async (
                Guid serverId,
                CreateChannelCommand command,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command with { ServerId = serverId }, cancellationToken);
                return result.ToHttpResult(http,
                    channel => Results.Created(
                        $"/api/v1/servers/{serverId}/channels/{channel.Id}",
                        channel));
            })
            .RequireAuthorization()
            .Produces<ChannelResult>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("CreateChannel")
            .WithSummary("POST /api/v1/servers/{serverId}/channels")
            .WithDescription("Creates a text or voice channel. Requires Admin or Owner role.");

        group.MapDelete("/{channelId:guid}",
            async (
                Guid serverId,
                Guid channelId,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new DeleteChannelCommand(serverId, channelId),
                    cancellationToken);
                return result.ToHttpResult(http, Results.NoContent);
            })
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("DeleteChannel")
            .WithSummary("DELETE /api/v1/servers/{serverId}/channels/{channelId}")
            .WithDescription("Deletes a channel and its messages. Requires Admin or Owner role.");

        return endpoints;
    }
}
