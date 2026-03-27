using BahadirMakina.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BahadirMakina.Infrastructure.Data.Configurations;

public class KullaniciConfiguration : IEntityTypeConfiguration<Kullanici>
{
    public void Configure(EntityTypeBuilder<Kullanici> builder)
    {
        builder.ToTable("Kullanicilar");

        builder.HasKey(k => k.Id);

        builder.Property(k => k.KullaniciAdi)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(k => k.KullaniciAdi)
            .IsUnique();

        builder.Property(k => k.AdSoyad)
            .HasMaxLength(200);

        builder.Property(k => k.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(k => k.Rol)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(k => k.Aktif)
            .IsRequired();
        builder.HasMany(k => k.IsAdimiLogs)
            .WithOne(l => l.Kullanici)
            .HasForeignKey(l => l.KullaniciId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(k => k.KaliteKontroller)
            .WithOne(q => q.KontrolEdenKullanici)
            .HasForeignKey(q => q.KontrolEdenKullaniciId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(k => k.DepoHareketleri)
            .WithOne(h => h.Kullanici)
            .HasForeignKey(h => h.KullaniciId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}