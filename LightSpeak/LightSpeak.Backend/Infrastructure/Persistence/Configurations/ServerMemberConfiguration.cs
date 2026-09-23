namespace LightSpeak.Backend.Infrastructure.Persistence.Configurations;

using LightSpeak.Backend.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class ServerMemberConfiguration : IEntityTypeConfiguration<ServerMember>
{
    public void Configure(EntityTypeBuilder<ServerMember> builder)
    {
        builder.HasKey(sm => new { sm.ServerId, sm.UserId });

        builder.Property(sm => sm.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(sm => sm.JoinedAt).IsRequired();

        builder.HasOne(sm => sm.Server)
            .WithMany(s => s.Members)
            .HasForeignKey(sm => sm.ServerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sm => sm.User)
            .WithMany(u => u.ServerMembers)
            .HasForeignKey(sm => sm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("ServerMembers");
    }
}
