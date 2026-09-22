namespace LightSpeak.Backend.Features.Auth.DTOs;

public sealed record AuthResponse(
    Guid UserId,
    string Email,
    string FullName,
    string AccessToken,
    string RefreshToken);
