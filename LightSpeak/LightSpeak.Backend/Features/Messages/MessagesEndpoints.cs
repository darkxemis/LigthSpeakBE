namespace LightSpeak.Backend.Features.Messages;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Messages.DeleteMessage;
using LightSpeak.Backend.Features.Messages.DTOs;
using LightSpeak.Backend.Features.Messages.EditMessage;
using LightSpeak.Backend.Features.Messages.GetChannelMessages;
using LightSpeak.Backend.Infrastructure.Middleware;
using MediatR;

public static class MessagesEndpoints
{
    public static IEndpointRouteBuilder MapMessagesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/channels/{channelId:guid}/messages").WithTags("Messages");

        group.MapGet("/",
            async (
                Guid channelId,
                Guid? beforeId,
                int? take,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new GetChannelMessagesQuery(channelId, beforeId, take ?? 50),
                    cancellationToken);
                return result.ToHttpResult(http, Results.Ok);
            })
            .RequireAuthorization()
            .Produces<MessagePageResult>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("GetChannelMessages")
            .WithSummary("GET /api/v1/channels/{channelId}/messages")
            .WithDescription(
                "Returns a page of channel history ordered chronologically. " +
                "Use beforeId (message id) to load older messages; take defaults to 50 (max 100).");

        group.MapPut("/{messageId:guid}",
            async (
                Guid channelId,
                Guid messageId,
                EditMessageCommand command,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    command with { ChannelId = channelId, MessageId = messageId },
                    cancellationToken);
                return result.ToHttpResult(http, Results.Ok);
            })
            .RequireAuthorization()
            .Produces<MessageResult>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("EditMessage")
            .WithSummary("PUT /api/v1/channels/{channelId}/messages/{messageId}")
            .WithDescription("Edits your own message. Author only.");

        group.MapDelete("/{messageId:guid}",
            async (
                Guid channelId,
                Guid messageId,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new DeleteMessageCommand(channelId, messageId),
                    cancellationToken);
                return result.ToHttpResult(http, Results.NoContent);
            })
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("DeleteMessage")
            .WithSummary("DELETE /api/v1/channels/{channelId}/messages/{messageId}")
            .WithDescription("Deletes a message. Allowed for the author or server Admin/Owner.");

        return endpoints;
    }
}
