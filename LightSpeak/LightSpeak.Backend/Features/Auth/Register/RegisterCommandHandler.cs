namespace LightSpeak.Backend.Features.Auth.Register;

using LightSpeak.Backend.Common.Constants;
using LightSpeak.Backend.Common.Entities;
using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class RegisterCommandHandler(
    IApplicationDbContext db,
    IPasswordHasher passwordHasher
) : IRequestHandler<RegisterCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await db.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailExists)
        {
            return Result<Guid>.Failure(Error.EmailAlreadyExists());
        }

        var memberRole = await db.Roles
            .FirstOrDefaultAsync(r => r.Name == RoleNames.Member, cancellationToken);

        if (memberRole is null)
        {
            memberRole = Role.Create(RoleNames.Member);
            await db.Roles.AddAsync(memberRole, cancellationToken);
        }

        var user = User.Create(
            request.Email,
            passwordHasher.Hash(request.Password),
            request.FirstName,
            request.LastName);

        user.UserRoles.Add(UserRole.Create(user.Id, memberRole.Id));

        await db.Users.AddAsync(user, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(user.Id);
    }
}
