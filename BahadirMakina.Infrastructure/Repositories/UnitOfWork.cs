using BahadirMakina.Domain.Entities;
using BahadirMakina.Domain.Interfaces;
using BahadirMakina.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace BahadirMakina.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        Kullanicilar = new GenericRepository<Kullanici>(_context);
        Depolar = new GenericRepository<Depo>(_context);
        Urunler = new GenericRepository<Urun>(_context);
        Stoklar = new GenericRepository<Stok>(_context);
        Istasyonlar = new GenericRepository<Istasyon>(_context);
        UrunAgaciAdimlari = new GenericRepository<UrunAgaciAdim>(_context);
        UrunAgaciGerekliBilesenler = new GenericRepository<UrunAgaciGerekliBilesen>(_context);
        IsEmirleri = new GenericRepository<IsEmri>(_context);
        IsAdimlari = new GenericRepository<IsAdimi>(_context);
        IsAdimiLoglari = new GenericRepository<IsAdimiLog>(_context);
        DepoHareketleri = new GenericRepository<DepoHareket>(_context);
        Hurdalar = new GenericRepository<Hurda>(_context);
    }

    public IGenericRepository<Kullanici> Kullanicilar { get; }
    public IGenericRepository<Depo> Depolar { get; }
    public IGenericRepository<Urun> Urunler { get; }
    public IGenericRepository<Stok> Stoklar { get; }
    public IGenericRepository<Istasyon> Istasyonlar { get; }
    public IGenericRepository<UrunAgaciAdim> UrunAgaciAdimlari { get; }
    public IGenericRepository<UrunAgaciGerekliBilesen> UrunAgaciGerekliBilesenler { get; }
    public IGenericRepository<IsEmri> IsEmirleri { get; }
    public IGenericRepository<IsAdimi> IsAdimlari { get; }
    public IGenericRepository<IsAdimiLog> IsAdimiLoglari { get; }
    public IGenericRepository<DepoHareket> DepoHareketleri { get; }
    public IGenericRepository<Hurda> Hurdalar { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}