namespace LightSpeak.Backend.Common.Interfaces;

public interface IJwtSettingsProvider
{
    int ExpirationInMinutes { get; }

    int RefreshTokenExpirationInDays { get; }
}
