namespace LightSpeak.Backend.Common.Interfaces;

public interface IJwtSettingsProvider
{
    int RefreshTokenExpirationInDays { get; }
}
