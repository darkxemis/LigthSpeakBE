namespace LightSpeak.Backend.Infrastructure.Persistence.Configurations;

using LightSpeak.Backend.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
{
    public void Configure(EntityTypeBuilder<UserSettings> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnOrder(0);
        builder.Property(s => s.CreatedAt).HasColumnOrder(1);
        builder.Property(s => s.UpdatedAt).HasColumnOrder(2);
        builder.Property(s => s.CreatedByUserId).HasColumnOrder(3);
        builder.Property(s => s.UpdatedByUserId).HasColumnOrder(4);

        builder.Property(s => s.UserId).IsRequired().HasColumnOrder(5);
        builder.Property(s => s.NoiseSuppressionEnabled).IsRequired().HasColumnOrder(6);
        builder.Property(s => s.EchoCancellationEnabled).IsRequired().HasColumnOrder(7);
        builder.Property(s => s.AutoGainControlEnabled).IsRequired().HasColumnOrder(8);
        builder.Property(s => s.SfxEnabled).IsRequired().HasColumnOrder(9);
        builder.Property(s => s.OutputVolume).IsRequired().HasColumnOrder(10);
        builder.Property(s => s.StartMuted).IsRequired().HasColumnOrder(11);
        builder.Property(s => s.PushToTalkEnabled).IsRequired().HasColumnOrder(12);
        builder.Property(s => s.PushToTalkKey).IsRequired().HasMaxLength(32).HasColumnOrder(13);
        builder.Property(s => s.DesktopNotificationsEnabled).IsRequired().HasColumnOrder(14);
        builder.Property(s => s.MessageSoundEnabled).IsRequired().HasColumnOrder(15);
        builder.Property(s => s.EnterToSendEnabled).IsRequired().HasColumnOrder(16);
        builder.Property(s => s.ShowTimestampsEnabled).IsRequired().HasColumnOrder(17);
        builder.Property(s => s.CompactMessagesEnabled).IsRequired().HasColumnOrder(18);
        builder.Property(s => s.ReducedMotionEnabled).IsRequired().HasColumnOrder(19);
        builder.Property(s => s.AccentColor).IsRequired().HasMaxLength(10).HasColumnOrder(20);

        builder.HasIndex(s => s.UserId).IsUnique();

        builder.HasOne(s => s.User)
            .WithOne()
            .HasForeignKey<UserSettings>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(
            "UserSettings",
            t => t.HasCheckConstraint(
                "CK_UserSettings_OutputVolume",
                "[OutputVolume] BETWEEN 0 AND 100"));
    }
}
