namespace LightSpeak.Backend.Features.Messages.EditMessage;

using FluentValidation;

public sealed class EditMessageCommandValidator : AbstractValidator<EditMessageCommand>
{
    public EditMessageCommandValidator()
    {
        RuleFor(x => x.ChannelId)
            .NotEmpty().WithMessage("Channel id is required.");

        RuleFor(x => x.MessageId)
            .NotEmpty().WithMessage("Message id is required.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message content is required.")
            .MaximumLength(4000).WithMessage("Message content must not exceed 4000 characters.");
    }
}
