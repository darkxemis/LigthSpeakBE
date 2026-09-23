namespace LightSpeak.Backend.Features.Users;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Users.DTOs;
using LightSpeak.Backend.Features.Users.DeleteProfileImage;
using LightSpeak.Backend.Features.Users.GetCurrentUser;
using LightSpeak.Backend.Features.Users.UploadProfileImage;
using LightSpeak.Backend.Infrastructure.Middleware;
using MediatR;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/users").WithTags("Users");

        group.MapGet("/me",
            async (
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetCurrentUserQuery(), cancellationToken);
                return result.ToHttpResult(http, Results.Ok);
            })
            .RequireAuthorization()
            .Produces<UserProfileResult>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("GetCurrentUser")
            .WithSummary("GET /api/v1/users/me")
            .WithDescription("Returns the profile of the currently authenticated user.");

        group.MapPost("/me/profile-image",
            async (
                IFormFile file,
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                await using var stream = file.OpenReadStream();
                var result = await sender.Send(
                    new UploadProfileImageCommand(stream, file.FileName),
                    cancellationToken);
                return result.ToHttpResult(http, Results.Ok);
            })
            .RequireAuthorization()
            .DisableAntiforgery()
            .Produces<UserProfileResult>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .WithName("UploadProfileImage")
            .WithSummary("POST /api/v1/users/me/profile-image")
            .WithDescription("Uploads a profile image for the currently authenticated user. Replaces any existing image.");

        group.MapDelete("/me/profile-image",
            async (
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteProfileImageCommand(), cancellationToken);
                return result.ToHttpResult(http, Results.Ok);
            })
            .RequireAuthorization()
            .Produces<UserProfileResult>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .WithName("DeleteProfileImage")
            .WithSummary("DELETE /api/v1/users/me/profile-image")
            .WithDescription("Deletes the profile image of the currently authenticated user.");

        return endpoints;
    }
}
