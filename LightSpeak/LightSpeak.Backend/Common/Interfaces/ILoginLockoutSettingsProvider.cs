namespace LightSpeak.Backend.Common.Interfaces;

public interface ILoginLockoutSettingsProvider
{
    int MaxFailedAttempts { get; }

    int LockoutMinutes { get; }
}
