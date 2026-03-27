using BahadirMakina.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BahadirMakina.Infrastructure.Data.Configurations;

public class KaliteKontrolConfiguration : IEntityTypeConfiguration<KaliteKontrol>
{
    public void Configure(EntityTypeBuilder<KaliteKontrol> builder)
    {
        builder.ToTable("KaliteKontroller");

        builder.HasKey(k => k.Id);

        builder.Property(k => k.KontrolEdilenMiktar)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(k => k.OnaylananMiktar)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(k => k.RedMiktar)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(k => k.Sonuc)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(k => k.RedSebebi)
            .HasMaxLength(500);

        builder.Property(k => k.Notlar)
            .HasMaxLength(1000);
        builder.HasOne(k => k.IsAdimi)
            .WithMany(a => a.KaliteKontroller)
            .HasForeignKey(k => k.IsAdimiId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(k => k.Urun)
            .WithMany()
            .HasForeignKey(k => k.UrunId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(k => k.Depo)
            .WithMany()
            .HasForeignKey(k => k.DepoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(k => k.KontrolEdenKullanici)
            .WithMany(u => u.KaliteKontroller)
            .HasForeignKey(k => k.KontrolEdenKullaniciId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}