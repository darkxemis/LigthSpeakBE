namespace LightSpeak.Backend.Infrastructure.Authentication;

using LightSpeak.Backend.Common.Interfaces;
using Microsoft.Extensions.Configuration;

public sealed class LoginLockoutSettingsProvider(
    IConfiguration configuration
) : ILoginLockoutSettingsProvider
{
    private readonly IConfigurationSection _section = configuration.GetSection("LoginLockout");

    public int MaxFailedAttempts => _section.GetValue<int>("MaxFailedAttempts");

    public int LockoutMinutes => _section.GetValue<int>("LockoutMinutes");
}
