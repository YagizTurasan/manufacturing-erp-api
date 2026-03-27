using BahadirMakina.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BahadirMakina.Infrastructure.Data.Configurations;

public class StokConfiguration : IEntityTypeConfiguration<Stok>
{
    public void Configure(EntityTypeBuilder<Stok> builder)
    {
        builder.ToTable("Stoklar");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Miktar)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.Durum)
            .IsRequired()
            .HasConversion<int>();
        builder.HasIndex(s => new { s.DepoId, s.UrunId });
        builder.HasOne(s => s.Depo)
            .WithMany(d => d.Stoklar)
            .HasForeignKey(s => s.DepoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Urun)
            .WithMany(u => u.Stoklar)
            .HasForeignKey(s => s.UrunId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.IsEmri)
            .WithMany()
            .HasForeignKey(s => s.IsEmriId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.IsAdimi)
            .WithMany()
            .HasForeignKey(s => s.IsAdimiId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}