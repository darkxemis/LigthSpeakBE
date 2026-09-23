namespace LightSpeak.Backend.Features.Servers.JoinServer;

using FluentValidation;

public sealed class JoinServerCommandValidator : AbstractValidator<JoinServerCommand>
{
    public JoinServerCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Invite code is required.")
            .MaximumLength(20).WithMessage("Invite code must not exceed 20 characters.");
    }
}
