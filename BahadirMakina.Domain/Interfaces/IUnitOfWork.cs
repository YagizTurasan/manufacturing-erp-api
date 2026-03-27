using BahadirMakina.Domain.Entities;

namespace BahadirMakina.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Kullanici> Kullanicilar { get; }
    IGenericRepository<Depo> Depolar { get; }
    IGenericRepository<Urun> Urunler { get; }
    IGenericRepository<Stok> Stoklar { get; }
    IGenericRepository<Istasyon> Istasyonlar { get; }
    IGenericRepository<UrunAgaciAdim> UrunAgaciAdimlari { get; }
    IGenericRepository<UrunAgaciGerekliBilesen> UrunAgaciGerekliBilesenler { get; }
    IGenericRepository<IsEmri> IsEmirleri { get; }
    IGenericRepository<IsAdimi> IsAdimlari { get; }
    IGenericRepository<IsAdimiLog> IsAdimiLoglari { get; }
    IGenericRepository<DepoHareket> DepoHareketleri { get; }
    IGenericRepository<Hurda> Hurdalar { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}