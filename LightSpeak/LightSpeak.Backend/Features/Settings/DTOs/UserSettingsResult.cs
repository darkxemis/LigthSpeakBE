namespace LightSpeak.Backend.Features.Settings.DTOs;

using LightSpeak.Backend.Dominio;

public sealed record UserSettingsResult(
    Guid UserId,
    bool NoiseSuppressionEnabled,
    bool EchoCancellationEnabled,
    bool AutoGainControlEnabled,
    bool SfxEnabled,
    int OutputVolume,
    bool StartMuted,
    bool PushToTalkEnabled,
    string PushToTalkKey,
    bool DesktopNotificationsEnabled,
    bool MessageSoundEnabled,
    bool EnterToSendEnabled,
    bool ShowTimestampsEnabled,
    bool CompactMessagesEnabled,
    bool ReducedMotionEnabled,
    string AccentColor)
{
    public static UserSettingsResult From(UserSettings settings)
    {
        return new UserSettingsResult(
            settings.UserId,
            settings.NoiseSuppressionEnabled,
            settings.EchoCancellationEnabled,
            settings.AutoGainControlEnabled,
            settings.SfxEnabled,
            settings.OutputVolume,
            settings.StartMuted,
            settings.PushToTalkEnabled,
            settings.PushToTalkKey,
            settings.DesktopNotificationsEnabled,
            settings.MessageSoundEnabled,
            settings.EnterToSendEnabled,
            settings.ShowTimestampsEnabled,
            settings.CompactMessagesEnabled,
            settings.ReducedMotionEnabled,
            settings.AccentColor);
    }
}
