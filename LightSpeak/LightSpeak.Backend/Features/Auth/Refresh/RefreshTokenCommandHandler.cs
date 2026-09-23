namespace LightSpeak.Backend.Features.Auth.Refresh;

using LightSpeak.Backend.Dominio;
using LightSpeak.Backend.Common.Interfaces;
using LightSpeak.Backend.Common.Results;
using LightSpeak.Backend.Features.Auth.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class RefreshTokenCommandHandler(
    IApplicationDbContext db,
    IJwtTokenGenerator jwtTokenGenerator,
    IJwtSettingsProvider jwtSettingsProvider
) : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.Token, cancellationToken);

        if (existingToken is null)
        {
            return Result<AuthResponse>.Failure(Error.InvalidRefreshToken());
        }

        if (!existingToken.IsActive)
        {
            return Result<AuthResponse>.Failure(Error.InvalidRefreshTokenInactive());
        }

        var user = await db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == existingToken.UserId, cancellationToken);

        if (user is null)
        {
            return Result<AuthResponse>.Failure(Error.InvalidRefreshToken());
        }

        var accessToken = jwtTokenGenerator.GenerateToken(user);
        var newRefreshToken = RefreshToken.Create(user.Id, jwtSettingsProvider.RefreshTokenExpirationInDays);

        existingToken.Revoke(newRefreshToken.Token);

        await db.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse(
            user.Id,
            user.Email,
            $"{user.FirstName} {user.LastName}",
            accessToken,
            newRefreshToken.Token,
            DateTime.UtcNow.AddMinutes(jwtSettingsProvider.ExpirationInMinutes)));
    }
}
