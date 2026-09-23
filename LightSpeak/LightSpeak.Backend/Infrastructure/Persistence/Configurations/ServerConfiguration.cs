namespace LightSpeak.Backend.Infrastructure.Persistence.Configurations;

using LightSpeak.Backend.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class ServerConfiguration : IEntityTypeConfiguration<Server>
{
    public void Configure(EntityTypeBuilder<Server> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnOrder(0);
        builder.Property(s => s.CreatedAt).HasColumnOrder(1);
        builder.Property(s => s.UpdatedAt).HasColumnOrder(2);
        builder.Property(s => s.CreatedByUserId).HasColumnOrder(3);
        builder.Property(s => s.UpdatedByUserId).HasColumnOrder(4);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(100).HasColumnOrder(5);
        builder.Property(s => s.OwnerId).IsRequired().HasColumnOrder(6);
        builder.Property(s => s.IconUrl).HasMaxLength(500).HasColumnOrder(7);
        builder.Property(s => s.InviteCode).IsRequired().HasMaxLength(10).HasColumnOrder(8);

        builder.HasIndex(s => s.InviteCode).IsUnique();
        builder.HasIndex(s => s.OwnerId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(s => s.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Servers");
    }
}
