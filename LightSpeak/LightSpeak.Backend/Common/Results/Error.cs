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

    public static Error ServerNotFound() =>
        new(HttpStatusCode.NotFound, ErrorTags.Servers.NotFound, "The requested server was not found.");

    public static Error ServerAlreadyMember() =>
        new(HttpStatusCode.Conflict, ErrorTags.Servers.AlreadyMember, "You are already a member of this server.");

    public static Error ServerNotMember() =>
        new(HttpStatusCode.NotFound, ErrorTags.Servers.NotMember, "You are not a member of this server.");

    public static Error ServerForbidden(string requiredRole) =>
        new(
            HttpStatusCode.Forbidden,
            ErrorTags.Servers.Forbidden,
            $"This action requires the '{requiredRole}' role.",
            new Dictionary<string, string> { ["requiredRole"] = requiredRole });

    public static Error ServerOwnerCannotLeave() =>
        new(HttpStatusCode.BadRequest, ErrorTags.Servers.OwnerCannotLeave, "The owner cannot leave the server. Delete it instead.");

    public static Error ServerInviteInvalid() =>
        new(HttpStatusCode.NotFound, ErrorTags.Servers.InviteInvalid, "The invite code is invalid or expired.");

    public static Error ServerMemberForbidden() =>
        new(
            HttpStatusCode.Forbidden,
            ErrorTags.Servers.MemberForbidden,
            "You cannot perform this action on this member.");

    public static Error ChannelNotFound() =>
        new(HttpStatusCode.NotFound, ErrorTags.Channels.NotFound, "The requested channel was not found.");

    public static Error MessageNotFound() =>
        new(HttpStatusCode.NotFound, ErrorTags.Messages.NotFound, "The requested message was not found.");

    public static Error MessageNotAuthor() =>
        new(HttpStatusCode.Forbidden, ErrorTags.Messages.NotAuthor, "You can only modify your own messages.");

    public static Error MessagesRateLimited(int retryAfterSeconds) =>
        new(
            (HttpStatusCode)429,
            ErrorTags.Messages.RateLimited,
            "You are sending messages too quickly.",
            new Dictionary<string, string> { ["retryAfterSeconds"] = retryAfterSeconds.ToString() });

    public static Error Validation(Dictionary<string, string> errors) =>
        new(HttpStatusCode.BadRequest, ErrorTags.Validation.Failed, "One or more validation errors occurred.", errors);
}
