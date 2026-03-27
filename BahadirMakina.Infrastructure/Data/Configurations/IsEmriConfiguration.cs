using BahadirMakina.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BahadirMakina.Infrastructure.Data.Configurations;

public class IsEmriConfiguration : IEntityTypeConfiguration<IsEmri>
{
    public void Configure(EntityTypeBuilder<IsEmri> builder)
    {
        builder.ToTable("IsEmirleri");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.IsEmriNo)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(i => i.IsEmriNo)
            .IsUnique();

        builder.Property(i => i.HedefMiktar)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.TamamlananMiktar)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.HurdaMiktar)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.Durum)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(i => i.Notlar)
            .HasMaxLength(2000);
        builder.HasOne(i => i.Urun)
            .WithMany()
            .HasForeignKey(i => i.UrunId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.OlusturanKullanici)
            .WithMany()
            .HasForeignKey(i => i.OlusturanKullaniciId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.IsAdimlari)
            .WithOne(a => a.IsEmri)
            .HasForeignKey(a => a.IsEmriId)
            .OnDelete(DeleteBehavior.Restrict); // CASCADE › RESTRICT

        builder.HasMany(i => i.DepoHareketleri)
            .WithOne(h => h.IsEmri)
            .HasForeignKey(h => h.IsEmriId)
            .OnDelete(DeleteBehavior.Restrict); // SET NULL › RESTRICT
    }
}