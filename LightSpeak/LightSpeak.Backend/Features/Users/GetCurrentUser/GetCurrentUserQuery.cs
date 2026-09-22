namespace LightSpeak.Backend.Features.Users.GetCurrentUser;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Users.DTOs;
using MediatR;

public sealed record GetCurrentUserQuery : IRequest<Result<UserProfileResult>>;
