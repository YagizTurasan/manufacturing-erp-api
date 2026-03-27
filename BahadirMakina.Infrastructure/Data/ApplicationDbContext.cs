using BahadirMakina.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BahadirMakina.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Kullanici> Kullanicilar { get; set; }
    public DbSet<Depo> Depolar { get; set; }
    public DbSet<Urun> Urunler { get; set; }
    public DbSet<Stok> Stoklar { get; set; }
    public DbSet<Istasyon> Istasyonlar { get; set; }
    public DbSet<UrunAgaciAdim> UrunAgaciAdimlari { get; set; }
    public DbSet<IsAdimiBilesen> IsAdimiBilesenleri { get; set; }
    public DbSet<IsEmri> IsEmirleri { get; set; }
    public DbSet<IsAdimi> IsAdimlari { get; set; }
    public DbSet<IsAdimiLog> IsAdimiLoglari { get; set; }
    public DbSet<DepoHareket> DepoHareketleri { get; set; }
    public DbSet<Hurda> Hurdalar { get; set; }
    public DbSet<KaliteKontrol> KaliteKontroller { get; set; }
    public DbSet<UrunAgaciGerekliBilesen> UrunAgaciGerekliBilesenler {  get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}