using BahadirMakina.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BahadirMakina.Infrastructure.Data.Configurations;

public class IstasyonConfiguration : IEntityTypeConfiguration<Istasyon>
{
    public void Configure(EntityTypeBuilder<Istasyon> builder)
    {
        builder.ToTable("Istasyonlar");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Ad)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Kod)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(i => i.Kod)
            .IsUnique();

        builder.Property(i => i.Tip)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(i => i.Durum)
            .IsRequired()
            .HasConversion<int>();
        builder.HasOne(i => i.Depo)
            .WithMany()
            .HasForeignKey(i => i.DepoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.AktifIsAdimi)
            .WithMany()
            .HasForeignKey(i => i.AktifIsAdimiId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(i => i.IsAdimlari)
            .WithOne(a => a.Istasyon)
            .HasForeignKey(a => a.IstasyonId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}