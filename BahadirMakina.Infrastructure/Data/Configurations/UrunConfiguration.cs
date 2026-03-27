using BahadirMakina.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BahadirMakina.Infrastructure.Data.Configurations;

public class UrunConfiguration : IEntityTypeConfiguration<Urun>
{
    public void Configure(EntityTypeBuilder<Urun> builder)
    {
        builder.ToTable("Urunler");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Ad)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Kod)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(u => u.Kod)
            .IsUnique();

        builder.Property(u => u.Tip)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(u => u.Birim)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Aciklama)
            .HasMaxLength(1000);

        builder.Property(u => u.MinimumStok)
            .HasColumnType("decimal(18,2)");
        builder.HasMany(u => u.Stoklar)
            .WithOne(s => s.Urun)
            .HasForeignKey(s => s.UrunId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.UrunAgaciAdimlari)
            .WithOne(a => a.Urun)
            .HasForeignKey(a => a.UrunId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.GerekliOlduguUrunler)
            .WithOne(b => b.Urun)
            .HasForeignKey(b => b.UrunId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.BilesenOlduguUrunler)
            .WithOne(b => b.BilesenUrun)
            .HasForeignKey(b => b.BilesenUrunId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}