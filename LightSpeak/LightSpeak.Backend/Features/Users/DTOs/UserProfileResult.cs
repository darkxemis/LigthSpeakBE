namespace LightSpeak.Backend.Features.Users.DTOs;

public sealed record UserProfileResult(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    IReadOnlyList<string> Roles,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string? ProfileImageUrl);
