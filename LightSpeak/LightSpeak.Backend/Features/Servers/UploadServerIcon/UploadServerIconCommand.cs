namespace LightSpeak.Backend.Features.Servers.UploadServerIcon;

using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Servers.DTOs;
using MediatR;

public sealed record UploadServerIconCommand(
    Guid ServerId,
    Stream FileStream,
    string FileName) : IRequest<Result<ServerResult>>;
