namespace LightSpeak.Backend.Infrastructure.Persistence;

using LightSpeak.Backend.Common.Constants;
using LightSpeak.Backend.Common.Entities;
using LightSpeak.Backend.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        if (!environment.IsDevelopment())
        {
            return;
        }

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (await db.Users.AnyAsync())
        {
            return;
        }

        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var adminRole = Role.Create(RoleNames.Admin);
        var memberRole = Role.Create(RoleNames.Member);

        var admin = User.Create(
            "admin@lightspeak.dev",
            passwordHasher.Hash("Admin123!"),
            "Admin",
            "LightSpeak");

        admin.UserRoles.Add(UserRole.Create(admin.Id, adminRole.Id));
        admin.UserRoles.Add(UserRole.Create(admin.Id, memberRole.Id));

        db.AddRange(adminRole, memberRole, admin);
        await db.SaveChangesAsync();
    }
}
