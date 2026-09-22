namespace LightSpeak.Backend.Common.Interfaces;

using LightSpeak.Backend.Common.Entities;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
