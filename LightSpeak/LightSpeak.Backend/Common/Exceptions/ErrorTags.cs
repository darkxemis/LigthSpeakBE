namespace LightSpeak.Backend.Common.Exceptions;

public static class ErrorTags
{
    public static class Auth
    {
        public const string InvalidCredentials = "auth.invalidCredentials";
        public const string InvalidRefreshToken = "auth.invalidRefreshToken";
        public const string AccountLocked = "auth.accountLocked";
    }

    public static class User
    {
        public const string NotFound = "user.notFound";
        public const string EmailAlreadyExists = "user.emailAlreadyExists";
        public const string InvalidImageFile = "user.invalidImageFile";
    }

    public static class Server
    {
        public const string InternalError = "server.internalError";
    }

    public static class Servers
    {
        public const string NotFound = "servers.notFound";
        public const string AlreadyMember = "servers.alreadyMember";
        public const string NotMember = "servers.notMember";
        public const string Forbidden = "servers.forbidden";
        public const string OwnerCannotLeave = "servers.ownerCannotLeave";
        public const string InviteInvalid = "servers.inviteInvalid";
        public const string MemberForbidden = "servers.memberForbidden";
    }

    public static class Channels
    {
        public const string NotFound = "channels.notFound";
    }

    public static class Messages
    {
        public const string NotFound = "messages.notFound";
        public const string NotAuthor = "messages.notAuthor";
        public const string RateLimited = "messages.rateLimited";
    }

    public static class Validation
    {
        public const string Failed = "validation.failed";
    }
}
