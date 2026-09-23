namespace LightSpeak.Backend.Features.Messages.GetChannelMessages;

using FluentValidation;

public sealed class GetChannelMessagesQueryValidator : AbstractValidator<GetChannelMessagesQuery>
{
    public GetChannelMessagesQueryValidator()
    {
        RuleFor(x => x.ChannelId)
            .NotEmpty().WithMessage("Channel id is required.");

        RuleFor(x => x.Take)
            .InclusiveBetween(1, 100).WithMessage("Take must be between 1 and 100.");
    }
}
