using BahadirMakina.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BahadirMakina.Infrastructure.Data.Configurations;

public class UrunAgaciGerekliBilesenConfiguration : IEntityTypeConfiguration<UrunAgaciGerekliBilesen>
{
    public void Configure(EntityTypeBuilder<UrunAgaciGerekliBilesen> builder)
    {
        builder.ToTable("UrunAgaciGerekliBilesenler");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Miktar)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        builder.HasIndex(b => new { b.UrunId, b.BilesenUrunId })
            .IsUnique();
        builder.HasOne(b => b.Urun)
            .WithMany(u => u.GerekliOlduguUrunler)
            .HasForeignKey(b => b.UrunId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.BilesenUrun)
            .WithMany(u => u.BilesenOlduguUrunler)
            .HasForeignKey(b => b.BilesenUrunId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}