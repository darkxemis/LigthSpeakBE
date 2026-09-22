namespace LightSpeak.Backend.Features.Users.GetCurrentUser;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetCurrentUserQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser
) : IRequestHandler<GetCurrentUserQuery, Result<UserProfileResult>>
{
    public async Task<Result<UserProfileResult>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var profile = await db.Users
            .AsNoTracking()
            .Where(u => u.Id == currentUser.UserId)
            .Select(u => new UserProfileResult(
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                u.CreatedAt,
                u.UpdatedAt,
                u.ProfileImageUrl))
            .FirstOrDefaultAsync(cancellationToken);

        return profile is null
            ? Result<UserProfileResult>.Failure(Error.UserNotFound())
            : Result<UserProfileResult>.Success(profile);
    }
}
