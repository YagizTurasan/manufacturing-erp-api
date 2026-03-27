using BahadirMakina.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BahadirMakina.Infrastructure.Data.Configurations;

public class HurdaConfiguration : IEntityTypeConfiguration<Hurda>
{
    public void Configure(EntityTypeBuilder<Hurda> builder)
    {
        builder.ToTable("Hurdalar");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Miktar)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(h => h.Sebep)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(h => h.TahminiMaliyetKaybi)
            .HasColumnType("decimal(18,2)");
        builder.HasOne(h => h.Urun)
            .WithMany()
            .HasForeignKey(h => h.UrunId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.IsEmri)
            .WithMany()
            .HasForeignKey(h => h.IsEmriId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.IsAdimi)
            .WithMany()
            .HasForeignKey(h => h.IsAdimiId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(h => h.KaliteKontrol)
            .WithMany()
            .HasForeignKey(h => h.KaliteKontrolId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.KaynakDepo)
            .WithMany()
            .HasForeignKey(h => h.KaynakDepoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}