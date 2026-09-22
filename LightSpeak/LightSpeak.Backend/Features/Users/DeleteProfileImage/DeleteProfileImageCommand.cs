namespace LightSpeak.Backend.Features.Users.DeleteProfileImage;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Users.DTOs;
using MediatR;

public sealed record DeleteProfileImageCommand : IRequest<Result<UserProfileResult>>;
