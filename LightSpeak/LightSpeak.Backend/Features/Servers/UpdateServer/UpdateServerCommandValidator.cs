namespace LightSpeak.Backend.Features.Servers.UpdateServer;

using FluentValidation;

public sealed class UpdateServerCommandValidator : AbstractValidator<UpdateServerCommand>
{
    public UpdateServerCommandValidator()
    {
        RuleFor(x => x.ServerId)
            .NotEmpty().WithMessage("Server id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Server name is required.")
            .MinimumLength(2).WithMessage("Server name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Server name must not exceed 100 characters.");
    }
}
