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

    public static class Validation
    {
        public const string Failed = "validation.failed";
    }
}
