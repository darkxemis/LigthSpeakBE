namespace LightSpeak.Backend.Features.Users.UploadProfileImage;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Users.DTOs;
using MediatR;

public sealed record UploadProfileImageCommand(
    Stream FileStream,
    string FileName) : IRequest<Result<UserProfileResult>>;
