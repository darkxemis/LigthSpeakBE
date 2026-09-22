namespace LightSpeak.Backend.Features.Auth.Refresh;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Auth.DTOs;
using MediatR;

public sealed record RefreshTokenCommand(string Token) : IRequest<Result<AuthResponse>>;
