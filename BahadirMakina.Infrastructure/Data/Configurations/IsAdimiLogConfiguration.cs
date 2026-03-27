using BahadirMakina.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BahadirMakina.Infrastructure.Data.Configurations;

public class IsAdimiLogConfiguration : IEntityTypeConfiguration<IsAdimiLog>
{
    public void Configure(EntityTypeBuilder<IsAdimiLog> builder)
    {
        builder.ToTable("IsAdimiLoglari");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.EskiDurum)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(l => l.YeniDurum)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(l => l.Aciklama)
            .HasMaxLength(1000);
        builder.HasOne(l => l.IsAdimi)
            .WithMany(a => a.Loglar)
            .HasForeignKey(l => l.IsAdimiId)
            .OnDelete(DeleteBehavior.Restrict); // CASCADE › RESTRICT

        builder.HasOne(l => l.Kullanici)
            .WithMany(k => k.IsAdimiLogs)
            .HasForeignKey(l => l.KullaniciId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}