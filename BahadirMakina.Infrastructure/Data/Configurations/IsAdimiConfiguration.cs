using BahadirMakina.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BahadirMakina.Infrastructure.Data.Configurations;

public class IsAdimiConfiguration : IEntityTypeConfiguration<IsAdimi>
{
    public void Configure(EntityTypeBuilder<IsAdimi> builder)
    {
        builder.ToTable("IsAdimlari");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Sira)
            .IsRequired();

        builder.Property(a => a.Tanim)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Durum)
            .IsRequired()
            .HasConversion<int>();
        builder.Property(a => a.HedefMiktar)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(a => a.TamamlananMiktar)
            .HasColumnType("decimal(18,2)");

        builder.Property(a => a.KaliteKontrolGerekli)
            .IsRequired();
        builder.HasOne(a => a.IsEmri)
            .WithMany(i => i.IsAdimlari)
            .HasForeignKey(a => a.IsEmriId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Istasyon)
            .WithMany(i => i.IsAdimlari)
            .HasForeignKey(a => a.IstasyonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.SorumluKullanici)
            .WithMany()
            .HasForeignKey(a => a.SorumluKullaniciId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.OncekiAdim)
            .WithMany(a => a.SonrakiAdimlar)
            .HasForeignKey(a => a.OncekiAdimId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Loglar)
            .WithOne(l => l.IsAdimi)
            .HasForeignKey(l => l.IsAdimiId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.GirdiDepo)
            .WithMany()
            .HasForeignKey(a => a.GirdiDepoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CiktiDepo)
            .WithMany()
            .HasForeignKey(a => a.CiktiDepoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.GirdiUrun)
            .WithMany()
            .HasForeignKey(a => a.GirdiUrunId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CiktiUrun)
            .WithMany()
            .HasForeignKey(a => a.CiktiUrunId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.KaliteKontroller)
            .WithOne(k => k.IsAdimi)
            .HasForeignKey(k => k.IsAdimiId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.DepoHareketleri)
            .WithOne(h => h.IsAdimi)
            .HasForeignKey(h => h.IsAdimiId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.GerekliBilesenler)
            .WithOne(b => b.IsAdimi)
            .HasForeignKey(b => b.IsAdimiId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}