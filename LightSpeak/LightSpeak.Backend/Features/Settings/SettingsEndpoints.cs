namespace LightSpeak.Backend.Features.Settings;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Settings.DTOs;
using LightSpeak.Backend.Features.Settings.GetUserSettings;
using LightSpeak.Backend.Features.Settings.UpdateUserSettings;
using LightSpeak.Backend.Infrastructure.Middleware;
using MediatR;

public static class SettingsEndpoints
{
    public static IEndpointRouteBuilder MapSettingsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/users/me/settings").WithTags("UserSettings");

        group.MapGet("/",
            async (
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetUserSettingsQuery(), cancellationToken);
                return result.ToHttpResult(http, Results.Ok);
            })
            .RequireAuthorization()
            .Produces<UserSettingsResult>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("GetUserSettings")
            .WithSummary("GET /api/v1/users/me/settings")
            .WithDescription("Returns the application settings of the currently authenticated user.");

        group.MapPatch("/",
            async (
                UpdateUserSettingsCommand command,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return result.ToHttpResult(http, Results.Ok);
            })
            .RequireAuthorization()
            .Produces<UserSettingsResult>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("UpdateUserSettings")
            .WithSummary("PATCH /api/v1/users/me/settings")
            .WithDescription("Partially updates the application settings of the currently authenticated user. Only the provided fields are changed.");

        return endpoints;
    }
}
