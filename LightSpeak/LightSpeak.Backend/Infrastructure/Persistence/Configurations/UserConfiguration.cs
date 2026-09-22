namespace LightSpeak.Backend.Infrastructure.Persistence.Configurations;

using LightSpeak.Backend.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnOrder(0);
        builder.Property(u => u.CreatedAt).HasColumnOrder(1);
        builder.Property(u => u.UpdatedAt).HasColumnOrder(2);
        builder.Property(u => u.CreatedByUserId).HasColumnOrder(3);
        builder.Property(u => u.UpdatedByUserId).HasColumnOrder(4);

        builder.Property(u => u.Email).IsRequired().HasMaxLength(200).HasColumnOrder(5);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash).IsRequired().HasColumnOrder(6);
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100).HasColumnOrder(7);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100).HasColumnOrder(8);
        builder.Property(u => u.FailedLoginAttempts).IsRequired().HasColumnOrder(9);
        builder.Property(u => u.LockoutEnd).HasColumnOrder(10);
        builder.Property(u => u.ProfileImageUrl).HasMaxLength(500).HasColumnOrder(11);

        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId);

        builder.ToTable("Users");
    }
}
