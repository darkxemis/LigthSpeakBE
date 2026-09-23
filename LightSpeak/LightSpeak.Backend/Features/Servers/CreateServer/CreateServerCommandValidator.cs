namespace LightSpeak.Backend.Features.Servers.CreateServer;

using FluentValidation;

public sealed class CreateServerCommandValidator : AbstractValidator<CreateServerCommand>
{
    public CreateServerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Server name is required.")
            .MinimumLength(2).WithMessage("Server name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Server name must not exceed 100 characters.");
    }
}
