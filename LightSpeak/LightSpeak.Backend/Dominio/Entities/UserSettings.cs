namespace LightSpeak.Backend.Dominio;

public sealed class UserSettings : Entity
{
    public const string DefaultAccentColor = "cyan";
    public const string DefaultPushToTalkKey = "Space";

    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public bool NoiseSuppressionEnabled { get; private set; }
    public bool EchoCancellationEnabled { get; private set; }
    public bool AutoGainControlEnabled { get; private set; }
    public bool SfxEnabled { get; private set; }
    public int OutputVolume { get; private set; }
    public bool StartMuted { get; private set; }
    public bool PushToTalkEnabled { get; private set; }
    public string PushToTalkKey { get; private set; } = default!;
    public bool DesktopNotificationsEnabled { get; private set; }
    public bool MessageSoundEnabled { get; private set; }
    public bool EnterToSendEnabled { get; private set; }
    public bool ShowTimestampsEnabled { get; private set; }
    public bool CompactMessagesEnabled { get; private set; }
    public bool ReducedMotionEnabled { get; private set; }
    public string AccentColor { get; private set; } = default!;

    private UserSettings()
    {
    }

    public static UserSettings CreateDefault(Guid userId)
    {
        return new UserSettings
        {
            UserId = userId,
            NoiseSuppressionEnabled = true,
            EchoCancellationEnabled = true,
            AutoGainControlEnabled = true,
            SfxEnabled = true,
            OutputVolume = 100,
            StartMuted = false,
            PushToTalkEnabled = false,
            PushToTalkKey = DefaultPushToTalkKey,
            DesktopNotificationsEnabled = false,
            MessageSoundEnabled = true,
            EnterToSendEnabled = true,
            ShowTimestampsEnabled = true,
            CompactMessagesEnabled = false,
            ReducedMotionEnabled = false,
            AccentColor = DefaultAccentColor
        };
    }

    public void Apply(
        bool? noiseSuppressionEnabled = null,
        bool? echoCancellationEnabled = null,
        bool? autoGainControlEnabled = null,
        bool? sfxEnabled = null,
        int? outputVolume = null,
        bool? startMuted = null,
        bool? pushToTalkEnabled = null,
        string? pushToTalkKey = null,
        bool? desktopNotificationsEnabled = null,
        bool? messageSoundEnabled = null,
        bool? enterToSendEnabled = null,
        bool? showTimestampsEnabled = null,
        bool? compactMessagesEnabled = null,
        bool? reducedMotionEnabled = null,
        string? accentColor = null)
    {
        if (noiseSuppressionEnabled is { } noise) NoiseSuppressionEnabled = noise;
        if (echoCancellationEnabled is { } echo) EchoCancellationEnabled = echo;
        if (autoGainControlEnabled is { } gain) AutoGainControlEnabled = gain;
        if (sfxEnabled is { } sfx) SfxEnabled = sfx;
        if (outputVolume is { } volume) OutputVolume = volume;
        if (startMuted is { } muted) StartMuted = muted;
        if (pushToTalkEnabled is { } ptt) PushToTalkEnabled = ptt;
        if (pushToTalkKey is { } key) PushToTalkKey = key;
        if (desktopNotificationsEnabled is { } desktop) DesktopNotificationsEnabled = desktop;
        if (messageSoundEnabled is { } sound) MessageSoundEnabled = sound;
        if (enterToSendEnabled is { } enter) EnterToSendEnabled = enter;
        if (showTimestampsEnabled is { } timestamps) ShowTimestampsEnabled = timestamps;
        if (compactMessagesEnabled is { } compact) CompactMessagesEnabled = compact;
        if (reducedMotionEnabled is { } motion) ReducedMotionEnabled = motion;
        if (accentColor is { } accent) AccentColor = accent;

        UpdatedAt = DateTime.UtcNow;
    }
}
