namespace LightSpeak.Backend.Features.Auth;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Auth.DTOs;
using LightSpeak.Backend.Features.Auth.Login;
using LightSpeak.Backend.Features.Auth.Refresh;
using LightSpeak.Backend.Features.Auth.Register;
using LightSpeak.Backend.Infrastructure.Middleware;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/auth").WithTags("Auth");

        group.MapPost("/register",
            async (
                ISender sender,
                [FromBody] RegisterCommand command,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return ToHttpResult(http, result,
                    userId => Results.Created($"/api/v1/auth/register/{userId}", new { userId }));
            })
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status409Conflict)
            .WithName("Register")
            .WithSummary("POST /api/v1/auth/register")
            .WithDescription("Registers a new user in the system. Open endpoint for development.");

        group.MapPost("/login",
            async (
                ISender sender,
                [FromBody] LoginQuery query,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(query, cancellationToken);
                return ToHttpResult(http, result, auth =>
                {
                    if (!ShouldSkipCookies(http))
                    {
                        SetAuthCookies(http, auth.AccessToken, auth.RefreshToken);
                    }

                    return Results.Ok(auth);
                });
            })
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiErrorResponse>(StatusCodes.Status403Forbidden)
            .WithName("Login")
            .WithSummary("POST /api/v1/auth/login")
            .WithDescription(
                "Authenticates a user and returns tokens in response body. " +
                "Cookies are set by default for web clients. " +
                "Mobile clients should add header 'X-Skip-Cookies: true'.");

        group.MapPost("/refresh",
            async (
                ISender sender,
                HttpContext http,
                CancellationToken cancellationToken) =>
            {
                var refreshToken = GetRefreshToken(http);
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return Results.Unauthorized();
                }

                var result = await sender.Send(new RefreshTokenCommand(refreshToken), cancellationToken);
                return ToHttpResult(http, result, auth =>
                {
                    if (!ShouldSkipCookies(http))
                    {
                        SetAuthCookies(http, auth.AccessToken, auth.RefreshToken);
                    }

                    return Results.Ok(auth);
                });
            })
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .WithName("Refresh")
            .WithSummary("POST /api/v1/auth/refresh")
            .WithDescription(
                "Refreshes tokens. Reads refresh token from cookie or 'X-Refresh-Token' header. " +
                "Cookies updated by default. Mobile clients should add 'X-Skip-Cookies: true'.");

        group.MapPost("/logout",
            (HttpContext http) =>
            {
                ClearAuthCookies(http);
                return Results.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .WithName("Logout")
            .WithSummary("POST /api/v1/auth/logout")
            .WithDescription("Clears authentication cookies. Mobile clients discard tokens locally.");

        return endpoints;
    }

    private static IResult ToHttpResult<T>(
        HttpContext http,
        Result<T> result,
        Func<T, IResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess(result.Value!);
        }

        var error = result.Error!;

        return Results.Json(
            new ApiErrorResponse(error.Code, error.Message, http.TraceIdentifier, error.Metadata),
            statusCode: (int)error.StatusCode);
    }

    private static bool ShouldSkipCookies(HttpContext http)
    {
        return http.Request.Headers.TryGetValue("X-Skip-Cookies", out var value) &&
               bool.TryParse(value.ToString(), out var skipCookies) &&
               skipCookies;
    }

    private static void SetAuthCookies(HttpContext http, string accessToken, string refreshToken)
    {
        var config = http.RequestServices.GetRequiredService<IConfiguration>();
        var accessMinutes = config.GetValue<int>("JwtSettings:ExpirationInMinutes");
        var refreshDays = config.GetValue<int>("JwtSettings:RefreshTokenExpirationInDays");

        http.Response.Cookies.Append("accessToken", accessToken,
            CreateCookieOptions(DateTimeOffset.UtcNow.AddMinutes(accessMinutes)));
        http.Response.Cookies.Append("refreshToken", refreshToken,
            CreateCookieOptions(DateTimeOffset.UtcNow.AddDays(refreshDays)));
    }

    private static string? GetRefreshToken(HttpContext http)
    {
        if (http.Request.Cookies.TryGetValue("refreshToken", out var cookieToken))
        {
            return cookieToken;
        }

        if (http.Request.Headers.TryGetValue("X-Refresh-Token", out var headerToken))
        {
            return headerToken.ToString();
        }

        return null;
    }

    private static void ClearAuthCookies(HttpContext http)
    {
        var expiredOptions = CreateCookieOptions(DateTimeOffset.UtcNow.AddDays(-1));
        http.Response.Cookies.Delete("accessToken", expiredOptions);
        http.Response.Cookies.Delete("refreshToken", expiredOptions);
    }

    private static CookieOptions CreateCookieOptions(DateTimeOffset expires) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
        Path = "/",
        Expires = expires,
    };
}
