namespace LightSpeak.Backend.Features.Settings.GetUserSettings;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Dominio;
using LightSpeak.Backend.Features.Settings.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetUserSettingsQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser
) : IRequestHandler<GetUserSettingsQuery, Result<UserSettingsResult>>
{
    public async Task<Result<UserSettingsResult>> Handle(
        GetUserSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var settings = await db.UserSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == currentUser.UserId, cancellationToken);

        var value = settings ?? UserSettings.CreateDefault(currentUser.UserId);

        return Result<UserSettingsResult>.Success(UserSettingsResult.From(value));
    }
}
