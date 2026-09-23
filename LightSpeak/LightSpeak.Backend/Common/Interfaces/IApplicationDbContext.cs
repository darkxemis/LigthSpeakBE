namespace LightSpeak.Backend.Common.Interfaces;

using LightSpeak.Backend.Dominio;
using Microsoft.EntityFrameworkCore;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<Server> Servers { get; }
    DbSet<ServerMember> ServerMembers { get; }
    DbSet<Channel> Channels { get; }
    DbSet<Message> Messages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
