namespace LightSpeak.Backend.Infrastructure.Persistence.Configurations;

using LightSpeak.Backend.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class ChannelConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnOrder(0);
        builder.Property(c => c.CreatedAt).HasColumnOrder(1);
        builder.Property(c => c.UpdatedAt).HasColumnOrder(2);
        builder.Property(c => c.CreatedByUserId).HasColumnOrder(3);
        builder.Property(c => c.UpdatedByUserId).HasColumnOrder(4);

        builder.Property(c => c.ServerId).IsRequired().HasColumnOrder(5);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100).HasColumnOrder(6);
        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasColumnOrder(7);
        builder.Property(c => c.Position).IsRequired().HasColumnOrder(8);

        builder.HasIndex(c => new { c.ServerId, c.Position });

        builder.HasOne(c => c.Server)
            .WithMany(s => s.Channels)
            .HasForeignKey(c => c.ServerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Channels");
    }
}
