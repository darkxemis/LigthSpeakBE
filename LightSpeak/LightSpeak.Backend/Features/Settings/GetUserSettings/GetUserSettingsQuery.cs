namespace LightSpeak.Backend.Features.Settings.GetUserSettings;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Settings.DTOs;
using MediatR;

public sealed record GetUserSettingsQuery : IRequest<Result<UserSettingsResult>>;
