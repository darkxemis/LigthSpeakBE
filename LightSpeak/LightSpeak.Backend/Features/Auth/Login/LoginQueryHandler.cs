namespace LightSpeak.Backend.Features.Auth.Login;

using LightSpeak.Backend.Common.Entities;
using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Auth.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class LoginQueryHandler(
    IApplicationDbContext db,
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordHasher passwordHasher,
    IJwtSettingsProvider jwtSettingsProvider,
    ILoginLockoutSettingsProvider lockoutSettingsProvider
) : IRequestHandler<LoginQuery, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
        {
            return Result<AuthResponse>.Failure(Error.InvalidCredentials());
        }

        if (user.IsLockedOut())
        {
            return Result<AuthResponse>.Failure(Error.AccountLocked(user.LockoutEnd));
        }

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.RegisterFailedLogin(
                lockoutSettingsProvider.MaxFailedAttempts,
                TimeSpan.FromMinutes(lockoutSettingsProvider.LockoutMinutes)
            );
            await db.SaveChangesAsync(cancellationToken);
            return Result<AuthResponse>.Failure(Error.InvalidCredentials());
        }

        user.ResetLoginFailures();

        var accessToken = jwtTokenGenerator.GenerateToken(user);
        var refreshToken = RefreshToken.Create(user.Id, jwtSettingsProvider.RefreshTokenExpirationInDays);

        await db.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse(
            user.Id,
            user.Email,
            $"{user.FirstName} {user.LastName}",
            accessToken,
            refreshToken.Token));
    }
}
