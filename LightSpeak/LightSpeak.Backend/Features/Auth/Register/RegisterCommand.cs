namespace LightSpeak.Backend.Features.Auth.Register;

using LightSpeak.Backend.Common.Results;
using MediatR;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : IRequest<Result<Guid>>;
