namespace LightSpeak.Backend.Common.Interfaces;

using LightSpeak.Backend.Dominio;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
