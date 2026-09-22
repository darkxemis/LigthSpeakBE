namespace LightSpeak.Backend.Infrastructure.Services;

using System.Security.Claims;
using LightSpeak.Backend.Common.Interfaces;

public sealed class CurrentUserService(
    IHttpContextAccessor httpContextAccessor
) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid UserId
    {
        get
        {
            var userIdClaim = User?.FindFirstValue("sub");
            return string.IsNullOrEmpty(userIdClaim) ? Guid.Empty : Guid.Parse(userIdClaim);
        }
    }

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
