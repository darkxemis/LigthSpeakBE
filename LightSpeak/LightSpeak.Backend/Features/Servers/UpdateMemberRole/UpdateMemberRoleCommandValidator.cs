namespace LightSpeak.Backend.Features.Servers.UpdateMemberRole;

using FluentValidation;
using LightSpeak.Backend.Dominio;

public sealed class UpdateMemberRoleCommandValidator : AbstractValidator<UpdateMemberRoleCommand>
{
    public UpdateMemberRoleCommandValidator()
    {
        RuleFor(x => x.ServerId)
            .NotEmpty().WithMessage("Server id is required.");

        RuleFor(x => x.TargetUserId)
            .NotEmpty().WithMessage("Target user id is required.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Role must be a valid server role.")
            .Must(role => role != ServerRole.Owner).WithMessage("Ownership cannot be assigned through this endpoint.");
    }
}
