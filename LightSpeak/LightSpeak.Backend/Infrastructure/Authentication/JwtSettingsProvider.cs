namespace LightSpeak.Backend.Infrastructure.Authentication;

using LightSpeak.Backend.Common.Interfaces;
using Microsoft.Extensions.Options;

public sealed class JwtSettingsProvider(
    IOptions<JwtSettings> options
) : IJwtSettingsProvider
{
    private readonly JwtSettings _jwtSettings = options.Value;

    public int RefreshTokenExpirationInDays => _jwtSettings.RefreshTokenExpirationInDays;
}
