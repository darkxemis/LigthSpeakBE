namespace LightSpeak.Backend.Infrastructure.Authentication;

public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string Secret { get; init; } = default!;
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public int ExpirationInMinutes { get; init; }
    public int RefreshTokenExpirationInDays { get; init; }
}
