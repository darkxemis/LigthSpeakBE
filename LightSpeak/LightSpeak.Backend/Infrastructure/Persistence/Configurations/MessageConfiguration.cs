namespace LightSpeak.Backend.Infrastructure.Persistence.Configurations;

using LightSpeak.Backend.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnOrder(0);
        builder.Property(m => m.CreatedAt).HasColumnOrder(1);
        builder.Property(m => m.UpdatedAt).HasColumnOrder(2);
        builder.Property(m => m.CreatedByUserId).HasColumnOrder(3);
        builder.Property(m => m.UpdatedByUserId).HasColumnOrder(4);

        builder.Property(m => m.ChannelId).IsRequired().HasColumnOrder(5);
        builder.Property(m => m.AuthorId).IsRequired().HasColumnOrder(6);
        builder.Property(m => m.Content).IsRequired().HasMaxLength(4000).HasColumnOrder(7);
        builder.Property(m => m.EditedAt).HasColumnOrder(8);

        builder.HasIndex(m => new { m.ChannelId, m.CreatedAt });

        builder.HasOne(m => m.Channel)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChannelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Author)
            .WithMany()
            .HasForeignKey(m => m.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Messages");
    }
}
