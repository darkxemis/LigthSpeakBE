namespace LightSpeak.Backend.Features.Settings.UpdateUserSettings;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Settings.DTOs;
using MediatR;

public sealed record UpdateUserSettingsCommand(
    bool? NoiseSuppressionEnabled = null,
    bool? EchoCancellationEnabled = null,
    bool? AutoGainControlEnabled = null,
    bool? SfxEnabled = null,
    int? OutputVolume = null,
    bool? StartMuted = null,
    bool? PushToTalkEnabled = null,
    string? PushToTalkKey = null,
    bool? DesktopNotificationsEnabled = null,
    bool? MessageSoundEnabled = null,
    bool? EnterToSendEnabled = null,
    bool? ShowTimestampsEnabled = null,
    bool? CompactMessagesEnabled = null,
    bool? ReducedMotionEnabled = null,
    string? AccentColor = null) : IRequest<Result<UserSettingsResult>>;
