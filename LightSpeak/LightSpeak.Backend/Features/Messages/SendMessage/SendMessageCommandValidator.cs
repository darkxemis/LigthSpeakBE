namespace LightSpeak.Backend.Features.Messages.SendMessage;

using FluentValidation;

public sealed class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.ChannelId)
            .NotEmpty().WithMessage("Channel id is required.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message content is required.")
            .MaximumLength(4000).WithMessage("Message content must not exceed 4000 characters.");
    }
}
