using BahadirMakina.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BahadirMakina.Infrastructure.Data.Configurations;

public class IsAdimiBilesenConfiguration : IEntityTypeConfiguration<IsAdimiBilesen>
{
    public void Configure(EntityTypeBuilder<IsAdimiBilesen> builder)
    {
        builder.ToTable("IsAdimiBilesenleri");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Miktar)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        builder.HasOne(b => b.IsAdimi)
            .WithMany(a => a.GerekliBilesenler)
            .HasForeignKey(b => b.IsAdimiId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Urun)
            .WithMany() 
            .HasForeignKey(b => b.BilesenUrunId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(b => new { b.IsAdimiId, b.BilesenUrunId })
            .IsUnique();
    }
}