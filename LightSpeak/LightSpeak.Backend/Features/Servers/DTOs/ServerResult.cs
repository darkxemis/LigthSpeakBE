namespace LightSpeak.Backend.Features.Servers.DTOs;

using LightSpeak.Backend.Dominio;

public sealed record ServerResult(
    Guid Id,
    string Name,
    string? IconUrl,
    Guid OwnerId,
    string InviteCode,
    DateTime CreatedAt);

public sealed record ServerMemberResult(
    Guid UserId,
    string FirstName,
    string LastName,
    string? ProfileImageUrl,
    ServerRole Role,
    DateTime JoinedAt);
