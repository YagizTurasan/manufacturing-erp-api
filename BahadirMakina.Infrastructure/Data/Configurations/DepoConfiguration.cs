using BahadirMakina.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BahadirMakina.Infrastructure.Data.Configurations;

public class DepoConfiguration : IEntityTypeConfiguration<Depo>
{
    public void Configure(EntityTypeBuilder<Depo> builder)
    {
        builder.ToTable("Depolar");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Ad)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Kod)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(d => d.Kod)
            .IsUnique();

        builder.Property(d => d.Tip)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(d => d.Aciklama)
            .HasMaxLength(500);
        builder.HasMany(d => d.Stoklar)
            .WithOne(s => s.Depo)
            .HasForeignKey(s => s.DepoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.DepoHareketleri)
            .WithOne(h => h.KaynakDepo)
            .HasForeignKey(h => h.KaynakDepoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}