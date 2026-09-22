namespace LightSpeak.Backend.Features.Users.DeleteProfileImage;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteProfileImageCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser,
    IFileStorageService fileStorage
) : IRequestHandler<DeleteProfileImageCommand, Result<UserProfileResult>>
{
    public async Task<Result<UserProfileResult>> Handle(DeleteProfileImageCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == currentUser.UserId, cancellationToken);

        if (user is null)
        {
            return Result<UserProfileResult>.Failure(Error.UserNotFound());
        }

        if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
        {
            await fileStorage.DeleteProfileImageAsync(user.ProfileImageUrl, cancellationToken);
        }

        user.SetProfileImage(null);
        await db.SaveChangesAsync(cancellationToken);

        var profile = new UserProfileResult(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.UserRoles.Select(ur => ur.Role.Name).ToList(),
            user.CreatedAt,
            user.UpdatedAt,
            user.ProfileImageUrl);

        return Result<UserProfileResult>.Success(profile);
    }
}
