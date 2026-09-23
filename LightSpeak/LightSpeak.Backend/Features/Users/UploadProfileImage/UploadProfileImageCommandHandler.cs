namespace LightSpeak.Backend.Features.Users.UploadProfileImage;

using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class UploadProfileImageCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser,
    IFileStorageService fileStorage
) : IRequestHandler<UploadProfileImageCommand, Result<UserProfileResult>>
{
    public async Task<Result<UserProfileResult>> Handle(UploadProfileImageCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == currentUser.UserId, cancellationToken);

        if (user is null)
        {
            return Result<UserProfileResult>.Failure(Error.UserNotFound());
        }

        string imageUrl;

        try
        {
            imageUrl = await fileStorage.SaveProfileImageAsync(user.Id, request.FileStream, request.FileName, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            return Result<UserProfileResult>.Failure(Error.InvalidImageFile());
        }

        if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
        {
            await fileStorage.DeleteProfileImageAsync(user.ProfileImageUrl, cancellationToken);
        }

        user.SetProfileImage(imageUrl);
        await db.SaveChangesAsync(cancellationToken);

        return Result<UserProfileResult>.Success(ToProfile(user));
    }

    private static UserProfileResult ToProfile(LightSpeak.Backend.Dominio.User user) =>
        new(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.UserRoles.Select(ur => ur.Role.Name).ToList(),
            user.CreatedAt,
            user.UpdatedAt,
            user.ProfileImageUrl);
}
