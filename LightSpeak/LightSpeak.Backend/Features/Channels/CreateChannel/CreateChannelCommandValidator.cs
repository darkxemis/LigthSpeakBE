namespace LightSpeak.Backend.Features.Channels.CreateChannel;

using FluentValidation;
using LightSpeak.Backend.Dominio;

public sealed class CreateChannelCommandValidator : AbstractValidator<CreateChannelCommand>
{
    public CreateChannelCommandValidator()
    {
        RuleFor(x => x.ServerId)
            .NotEmpty().WithMessage("Server id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Channel name is required.")
            .MaximumLength(100).WithMessage("Channel name must not exceed 100 characters.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Channel type must be Text or Voice.");
    }
}
