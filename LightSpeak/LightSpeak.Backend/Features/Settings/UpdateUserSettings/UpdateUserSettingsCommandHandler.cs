namespace LightSpeak.Backend.Features.Settings.UpdateUserSettings;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using LightSpeak.Backend.Features.Settings.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class UpdateUserSettingsCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser
) : IRequestHandler<UpdateUserSettingsCommand, Result<UserSettingsResult>>
{
    public async Task<Result<UserSettingsResult>> Handle(
        UpdateUserSettingsCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var settings = await db.UserSettings
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

        if (settings is null)
        {
            if (!await db.Users.AnyAsync(u => u.Id == userId, cancellationToken))
            {
                return Result<UserSettingsResult>.Failure(Error.UserNotFound());
            }

            settings = UserSettings.CreateDefault(userId);
            await db.UserSettings.AddAsync(settings, cancellationToken);
        }

        settings.Apply(
            request.NoiseSuppressionEnabled,
            request.EchoCancellationEnabled,
            request.AutoGainControlEnabled,
            request.SfxEnabled,
            request.OutputVolume,
            request.StartMuted,
            request.PushToTalkEnabled,
            request.PushToTalkKey,
            request.DesktopNotificationsEnabled,
            request.MessageSoundEnabled,
            request.EnterToSendEnabled,
            request.ShowTimestampsEnabled,
            request.CompactMessagesEnabled,
            request.ReducedMotionEnabled,
            request.AccentColor);

        await db.SaveChangesAsync(cancellationToken);

        return Result<UserSettingsResult>.Success(UserSettingsResult.From(settings));
    }
}
