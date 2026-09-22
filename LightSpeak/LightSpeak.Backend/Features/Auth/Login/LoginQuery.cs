namespace LightSpeak.Backend.Features.Auth.Login;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Auth.DTOs;
using MediatR;

public sealed record LoginQuery(string Email, string Password) : IRequest<Result<AuthResponse>>;
