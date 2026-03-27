using BahadirMakina.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BahadirMakina.Infrastructure.Data.Configurations;

public class DepoHareketConfiguration : IEntityTypeConfiguration<DepoHareket>
{
    public void Configure(EntityTypeBuilder<DepoHareket> builder)
    {
        builder.ToTable("DepoHareketleri");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Miktar)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(h => h.Tip)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(h => h.Aciklama)
            .HasMaxLength(1000);
        builder.HasOne(h => h.IsEmri)
            .WithMany(i => i.DepoHareketleri)
            .HasForeignKey(h => h.IsEmriId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.Urun)
            .WithMany()
            .HasForeignKey(h => h.UrunId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.KaynakDepo)
            .WithMany(d => d.DepoHareketleri)
            .HasForeignKey(h => h.KaynakDepoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.HedefDepo)
            .WithMany()
            .HasForeignKey(h => h.HedefDepoId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(h => h.Istasyon)
            .WithMany()
            .HasForeignKey(h => h.IstasyonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.IsAdimi)
            .WithMany(a => a.DepoHareketleri)
            .HasForeignKey(h => h.IsAdimiId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.Kullanici)
            .WithMany(k => k.DepoHareketleri)
            .HasForeignKey(h => h.KullaniciId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}