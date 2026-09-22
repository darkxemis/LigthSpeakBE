namespace LightSpeak.Backend.Common.Results;

using System.Net;
using LightSpeak.Backend.Common.Exceptions;

public sealed record Error(
    HttpStatusCode StatusCode,
    string Code,
    string Message,
    Dictionary<string, string>? Metadata = null)
{
    public static Error InvalidCredentials() =>
        new(HttpStatusCode.Unauthorized, ErrorTags.Auth.InvalidCredentials, "Invalid email or password.");

    public static Error AccountLocked(DateTime? lockoutEnd) =>
        new(
            HttpStatusCode.Forbidden,
            ErrorTags.Auth.AccountLocked,
            $"Account is locked until {lockoutEnd:HH:mm:ss} UTC.",
            new Dictionary<string, string>
            {
                ["unlockAtUtc"] = lockoutEnd?.ToString("o") ?? string.Empty
            });

    public static Error InvalidRefreshToken() =>
        new(HttpStatusCode.Unauthorized, ErrorTags.Auth.InvalidRefreshToken, "Invalid refresh token.");

    public static Error InvalidRefreshTokenInactive() =>
        new(HttpStatusCode.Unauthorized, ErrorTags.Auth.InvalidRefreshToken, "Refresh token is expired or revoked.");

    public static Error EmailAlreadyExists() =>
        new(HttpStatusCode.Conflict, ErrorTags.User.EmailAlreadyExists, "An account with this email already exists.");

    public static Error UserNotFound() =>
        new(HttpStatusCode.NotFound, ErrorTags.User.NotFound, "The requested user was not found.");

    public static Error InvalidImageFile() =>
        new(HttpStatusCode.BadRequest, ErrorTags.User.InvalidImageFile, "Only jpg, jpeg, png, gif or webp images are allowed.");

    public static Error Validation(Dictionary<string, string> errors) =>
        new(HttpStatusCode.BadRequest, ErrorTags.Validation.Failed, "One or more validation errors occurred.", errors);
}
