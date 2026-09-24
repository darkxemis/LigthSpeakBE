namespace LightSpeak.Backend.Features.Settings.UpdateUserSettings;

using FluentValidation;

public sealed class UpdateUserSettingsCommandValidator : AbstractValidator<UpdateUserSettingsCommand>
{
    public UpdateUserSettingsCommandValidator()
    {
        When(
            x => x.OutputVolume is not null,
            () => RuleFor(x => x.OutputVolume)
                .Must(volume => volume is >= 0 and <= 100)
                .WithMessage("Output volume must be between 0 and 100."));

        When(
            x => x.PushToTalkKey is not null,
            () =>
            {
                RuleFor(x => x.PushToTalkKey)
                    .NotEmpty().WithMessage("Push to talk key is required.")
                    .MaximumLength(32).WithMessage("Push to talk key must not exceed 32 characters.")
                    .Matches("^[A-Za-z0-9]+$").WithMessage("Push to talk key must be a valid key code.");
            });

        When(
            x => x.AccentColor is not null,
            () => RuleFor(x => x.AccentColor)
                .Must(accent => accent is "cyan" or "lime" or "violet" or "rose")
                .WithMessage("Accent color must be cyan, lime, violet or rose."));
    }
}
