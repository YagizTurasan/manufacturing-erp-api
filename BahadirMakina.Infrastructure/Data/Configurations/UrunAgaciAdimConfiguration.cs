using BahadirMakina.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BahadirMakina.Infrastructure.Data.Configurations;

public class UrunAgaciAdimConfiguration : IEntityTypeConfiguration<UrunAgaciAdim>
{
    public void Configure(EntityTypeBuilder<UrunAgaciAdim> builder)
    {
        builder.ToTable("UrunAgaciAdimlari");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Sira)
            .IsRequired();
        builder.Property(a => a.IstasyonTipi)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.Tanim)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(a => a.KaliteKontrolGerekli)
            .IsRequired();
        builder.HasIndex(a => new { a.UrunId, a.Sira })
            .IsUnique();
        builder.HasOne(a => a.Urun)
            .WithMany(u => u.UrunAgaciAdimlari)
            .HasForeignKey(a => a.UrunId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CiktiUrun)
            .WithMany()
            .HasForeignKey(a => a.CiktiUrunId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.HedefDepo)
            .WithMany()
            .HasForeignKey(a => a.HedefDepoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}