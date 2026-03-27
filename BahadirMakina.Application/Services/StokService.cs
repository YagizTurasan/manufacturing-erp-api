using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.DTOs.Stok;
using BahadirMakina.Application.Interfaces;
using BahadirMakina.Domain.Entities;
using BahadirMakina.Domain.Enums;
using BahadirMakina.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BahadirMakina.Application.Services;

public class StokService : IStokService
{
    private readonly ApplicationDbContext _context;

    public StokService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ResultDto<List<StokDto>>> GetAllAsync()
    {
        try
        {
            var stoklar = await _context.Stoklar
                .Include(s => s.Urun)
                .Include(s => s.Depo)
                .Select(s => new StokDto
                {
                    Id = s.Id,
                    UrunAdi = s.Urun.Ad,
                    UrunKodu = s.Urun.Kod,
                    DepoAdi = s.Depo.Ad,
                    Miktar = s.Miktar,
                    Birim = s.Urun.Birim,
                    Durum = s.Durum.ToString(),
                    MinimumStok = s.Urun.MinimumStok,
                    DusukStok = s.Urun.MinimumStok.HasValue && s.Miktar < s.Urun.MinimumStok.Value
                })
                .ToListAsync();

            return ResultDto<List<StokDto>>.SuccessResult(stoklar);
        }
        catch (Exception ex)
        {
            return ResultDto<List<StokDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<StokDto>>> GetByDepoIdAsync(int depoId)
    {
        try
        {
            var stoklar = await _context.Stoklar
                .Include(s => s.Urun)
                .Include(s => s.Depo)
                .Where(s => s.DepoId == depoId)
                .Select(s => new StokDto
                {
                    Id = s.Id,
                    UrunAdi = s.Urun.Ad,
                    UrunKodu = s.Urun.Kod,
                    DepoAdi = s.Depo.Ad,
                    Miktar = s.Miktar,
                    Birim = s.Urun.Birim,
                    Durum = s.Durum.ToString(),
                    MinimumStok = s.Urun.MinimumStok,
                    DusukStok = s.Urun.MinimumStok.HasValue && s.Miktar < s.Urun.MinimumStok.Value
                })
                .ToListAsync();

            return ResultDto<List<StokDto>>.SuccessResult(stoklar);
        }
        catch (Exception ex)
        {
            return ResultDto<List<StokDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<StokDto>>> GetByUrunIdAsync(int urunId)
    {
        try
        {
            var stoklar = await _context.Stoklar
                .Include(s => s.Urun)
                .Include(s => s.Depo)
                .Where(s => s.UrunId == urunId)
                .Select(s => new StokDto
                {
                    Id = s.Id,
                    UrunAdi = s.Urun.Ad,
                    UrunKodu = s.Urun.Kod,
                    DepoAdi = s.Depo.Ad,
                    Miktar = s.Miktar,
                    Birim = s.Urun.Birim,
                    Durum = s.Durum.ToString(),
                    MinimumStok = s.Urun.MinimumStok,
                    DusukStok = s.Urun.MinimumStok.HasValue && s.Miktar < s.Urun.MinimumStok.Value
                })
                .ToListAsync();

            return ResultDto<List<StokDto>>.SuccessResult(stoklar);
        }
        catch (Exception ex)
        {
            return ResultDto<List<StokDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }
    public async Task<ResultDto<StokDto>> StokGirisiAsync(StokGirisiDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            Console.WriteLine($"\n=== STOK GÝRÝÞÝ ===");
            Console.WriteLine($"UrunId: {dto.UrunId}, DepoId: {dto.DepoId}, Miktar: {dto.Miktar}");

            var urun = await _context.Urunler.FindAsync(dto.UrunId);
            if (urun == null)
                return ResultDto<StokDto>.FailureResult("Ürün bulunamadý!");

            var depo = await _context.Depolar.FindAsync(dto.DepoId);
            if (depo == null)
                return ResultDto<StokDto>.FailureResult("Depo bulunamadý!");
            var mevcutStok = await _context.Stoklar
                .FirstOrDefaultAsync(s => s.UrunId == dto.UrunId && s.DepoId == dto.DepoId);

            if (mevcutStok != null)
            {
                Console.WriteLine($"Mevcut stok güncelleniyor: {mevcutStok.Miktar} + {dto.Miktar}");
                mevcutStok.Miktar += dto.Miktar;
                mevcutStok.Durum = StokDurum.Hazir;
                mevcutStok.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                Console.WriteLine("Yeni stok kaydý oluþturuluyor");
                mevcutStok = new Stok
                {
                    UrunId = dto.UrunId,
                    DepoId = dto.DepoId,
                    Miktar = dto.Miktar,
                    Durum = StokDurum.Hazir
                };
                await _context.Stoklar.AddAsync(mevcutStok);
            }
            var hareket = new DepoHareket
            {
                UrunId = dto.UrunId,
                HedefDepoId = dto.DepoId,
                KaynakDepoId = null,
                Miktar = dto.Miktar,
                Tip = DepoHareketTip.Giris,
                Aciklama = dto.Aciklama ?? "Manuel stok giriþi",
                IslemTarihi = DateTime.UtcNow,
                KullaniciId = dto.KullaniciId
            };
            await _context.DepoHareketleri.AddAsync(hareket);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            Console.WriteLine("? Stok giriþi tamamlandý\n");

            var result = new StokDto
            {
                Id = mevcutStok.Id,
                UrunAdi = urun.Ad,
                UrunKodu = urun.Kod,
                DepoAdi = depo.Ad,
                Miktar = mevcutStok.Miktar,
                Birim = urun.Birim,
                Durum = mevcutStok.Durum.ToString()
            };

            return ResultDto<StokDto>.SuccessResult(result, "Stok giriþi baþarýlý!");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"? Stok giriþi hatasý: {ex.Message}");
            return ResultDto<StokDto>.FailureResult($"Hata: {ex.Message}");
        }
    }
    public async Task<ResultDto<StokDto>> StokCikisiAsync(StokCikisiDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            Console.WriteLine($"\n=== STOK ÇIKIÞI ===");
            Console.WriteLine($"UrunId: {dto.UrunId}, DepoId: {dto.DepoId}, Miktar: {dto.Miktar}");

            var mevcutStok = await _context.Stoklar
                .Include(s => s.Urun)
                .Include(s => s.Depo)
                .FirstOrDefaultAsync(s => s.UrunId == dto.UrunId && s.DepoId == dto.DepoId);

            if (mevcutStok == null)
                return ResultDto<StokDto>.FailureResult("Stok bulunamadý!");

            if (mevcutStok.Miktar < dto.Miktar)
                return ResultDto<StokDto>.FailureResult($"Yetersiz stok! Mevcut: {mevcutStok.Miktar}");

            Console.WriteLine($"Stoktan düþülüyor: {mevcutStok.Miktar} - {dto.Miktar}");
            mevcutStok.Miktar -= dto.Miktar;
            mevcutStok.UpdatedAt = DateTime.UtcNow;
            if (mevcutStok.Miktar == 0)
            {
                Console.WriteLine($"?? Stok 0 oldu, kayýt siliniyor: ID={mevcutStok.Id}");
                _context.Stoklar.Remove(mevcutStok);
            }
            var hareket = new DepoHareket
            {
                UrunId = dto.UrunId,
                KaynakDepoId = dto.DepoId,
                HedefDepoId = null,
                Miktar = dto.Miktar,
                Tip = DepoHareketTip.Cikis,
                Aciklama = dto.Aciklama ?? "Manuel stok çýkýþý",
                IslemTarihi = DateTime.UtcNow,
                KullaniciId = dto.KullaniciId
            };
            await _context.DepoHareketleri.AddAsync(hareket);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            Console.WriteLine("? Stok çýkýþý tamamlandý\n");

            var result = new StokDto
            {
                Id = mevcutStok.Id,
                UrunAdi = mevcutStok.Urun.Ad,
                UrunKodu = mevcutStok.Urun.Kod,
                DepoAdi = mevcutStok.Depo.Ad,
                Miktar = mevcutStok.Miktar,
                Birim = mevcutStok.Urun.Birim,
                Durum = mevcutStok.Durum.ToString()
            };

            return ResultDto<StokDto>.SuccessResult(result, "Stok çýkýþý baþarýlý!");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"? Stok çýkýþý hatasý: {ex.Message}");
            return ResultDto<StokDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<StokDto>>> GetDusukStoklarAsync()
    {
        try
        {
            var stoklar = await _context.Stoklar
                .Include(s => s.Urun)
                .Include(s => s.Depo)
                .Where(s => s.Urun.MinimumStok.HasValue && s.Miktar < s.Urun.MinimumStok.Value)
                .Select(s => new StokDto
                {
                    Id = s.Id,
                    UrunAdi = s.Urun.Ad,
                    UrunKodu = s.Urun.Kod,
                    DepoAdi = s.Depo.Ad,
                    Miktar = s.Miktar,
                    Birim = s.Urun.Birim,
                    Durum = s.Durum.ToString(),
                    MinimumStok = s.Urun.MinimumStok,
                    DusukStok = true
                })
                .ToListAsync();

            return ResultDto<List<StokDto>>.SuccessResult(stoklar);
        }
        catch (Exception ex)
        {
            return ResultDto<List<StokDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }
    public async Task<ResultDto<bool>> StokTransferAsync(StokTransferDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            Console.WriteLine($"\n=== STOK TRANSFER ===");
            Console.WriteLine($"UrunId: {dto.UrunId}");
            Console.WriteLine($"Kaynak Depo: {dto.KaynakDepoId} › Hedef Depo: {dto.HedefDepoId}");
            Console.WriteLine($"Miktar: {dto.Miktar}");
            var kaynakStok = await _context.Stoklar
                .FirstOrDefaultAsync(s => s.DepoId == dto.KaynakDepoId && s.UrunId == dto.UrunId);

            if (kaynakStok == null || kaynakStok.Miktar < dto.Miktar)
            {
                Console.WriteLine("? Kaynak depoda yetersiz stok!");
                return ResultDto<bool>.FailureResult("Kaynak depoda yetersiz stok!");
            }
            kaynakStok.Miktar -= dto.Miktar;
            kaynakStok.UpdatedAt = DateTime.UtcNow;
            Console.WriteLine($"Kaynak stok güncellendi: Yeni Miktar = {kaynakStok.Miktar}");
            if (kaynakStok.Miktar == 0)
            {
                Console.WriteLine($"?? Kaynak stok 0 oldu, kayýt siliniyor: ID={kaynakStok.Id}");
                _context.Stoklar.Remove(kaynakStok);
            }
            var hedefStok = await _context.Stoklar
                .FirstOrDefaultAsync(s => s.DepoId == dto.HedefDepoId && s.UrunId == dto.UrunId);

            if (hedefStok != null)
            {
                hedefStok.Miktar += dto.Miktar;
                hedefStok.Durum = StokDurum.Hazir;
                hedefStok.UpdatedAt = DateTime.UtcNow;
                Console.WriteLine($"Hedef stok güncellendi: Yeni Miktar = {hedefStok.Miktar}");
            }
            else
            {
                var yeniStok = new Stok
                {
                    UrunId = dto.UrunId,
                    DepoId = dto.HedefDepoId,
                    Miktar = dto.Miktar,
                    Durum = StokDurum.Hazir
                };
                await _context.Stoklar.AddAsync(yeniStok);
                Console.WriteLine("Hedef depoda yeni stok oluþturuldu");
            }
            var hareket = new DepoHareket
            {
                UrunId = dto.UrunId,
                KaynakDepoId = dto.KaynakDepoId,
                HedefDepoId = dto.HedefDepoId,
                Miktar = dto.Miktar,
                Tip = DepoHareketTip.Transfer,
                Aciklama = dto.Aciklama ?? "Depolar arasý transfer",
                IslemTarihi = DateTime.UtcNow,
                KullaniciId = dto.KullaniciId
            };
            await _context.DepoHareketleri.AddAsync(hareket);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            Console.WriteLine("? Stok transfer tamamlandý\n");
            return ResultDto<bool>.SuccessResult(true, "Stok transferi baþarýyla yapýldý!");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"? Transfer hatasý: {ex.Message}");
            return ResultDto<bool>.FailureResult($"Hata: {ex.Message}");
        }
    }
    public async Task<ResultDto<bool>> StokSayimAsync(StokSayimDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            Console.WriteLine($"\n=== STOK SAYIM ===");
            Console.WriteLine($"UrunId: {dto.UrunId}, DepoId: {dto.DepoId}");
            Console.WriteLine($"Yeni Miktar: {dto.YeniMiktar}");

            var mevcutStok = await _context.Stoklar
                .FirstOrDefaultAsync(s => s.DepoId == dto.DepoId && s.UrunId == dto.UrunId);

            decimal eskiMiktar = mevcutStok?.Miktar ?? 0;
            decimal fark = dto.YeniMiktar - eskiMiktar;

            Console.WriteLine($"Eski Miktar: {eskiMiktar}");
            Console.WriteLine($"Fark: {fark}");

            if (mevcutStok != null)
            {
                mevcutStok.Miktar = dto.YeniMiktar;
                mevcutStok.Durum = StokDurum.Hazir;
                mevcutStok.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var yeniStok = new Stok
                {
                    UrunId = dto.UrunId,
                    DepoId = dto.DepoId,
                    Miktar = dto.YeniMiktar,
                    Durum = StokDurum.Hazir
                };
                await _context.Stoklar.AddAsync(yeniStok);
            }
            var hareket = new DepoHareket
            {
                UrunId = dto.UrunId,
                HedefDepoId = fark > 0 ? dto.DepoId : null,
                KaynakDepoId = fark < 0 ? dto.DepoId : null,
                Miktar = Math.Abs(fark),
                Tip = DepoHareketTip.Sayim,
                Aciklama = $"Stok sayýmý - {dto.Aciklama ?? $"Fark: {fark}"}",
                IslemTarihi = DateTime.UtcNow,
                KullaniciId = dto.KullaniciId
            };
            await _context.DepoHareketleri.AddAsync(hareket);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            Console.WriteLine("? Stok sayým tamamlandý\n");
            return ResultDto<bool>.SuccessResult(true, "Stok sayýmý baþarýyla yapýldý!");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"? Sayým hatasý: {ex.Message}");
            return ResultDto<bool>.FailureResult($"Hata: {ex.Message}");
        }
    }
    public async Task<ResultDto<bool>> StokOlusturVeyaGuncelleAsync(
        int? depoId,
        int urunId,
        decimal miktar,
        StokDurum durum,
        int? isEmriId = null,
        int? isAdimiId = null)
    {
        try
        {
            if (!depoId.HasValue)
            {
                Console.WriteLine("? Depo ID gerekli!");
                return ResultDto<bool>.FailureResult("Depo ID gerekli!");
            }

            Console.WriteLine($"\n=== STOK OLUÞTUR/GÜNCELLE ===");
            Console.WriteLine($"DepoId: {depoId}, UrunId: {urunId}, Miktar: {miktar}, Durum: {durum}");
            var mevcutStok = await _context.Stoklar
                .Where(s => s.DepoId == depoId && s.UrunId == urunId)
                .OrderByDescending(s => s.UpdatedAt)
                .FirstOrDefaultAsync();

            if (mevcutStok != null)
            {
                Console.WriteLine($"? Mevcut stok: ID={mevcutStok.Id}, Eski Miktar={mevcutStok.Miktar}");
                mevcutStok.Miktar += miktar;
                mevcutStok.Durum = durum;
                mevcutStok.UpdatedAt = DateTime.UtcNow;
                Console.WriteLine($"? Güncellendi: Yeni Miktar={mevcutStok.Miktar}");
            }
            else
            {
                var yeniStok = new Stok
                {
                    UrunId = urunId,
                    DepoId = depoId.Value,
                    Miktar = miktar,
                    Durum = durum
                };
                await _context.Stoklar.AddAsync(yeniStok);
                Console.WriteLine($"? Yeni stok oluþturuldu: Miktar={miktar}");
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("================================\n");

            return ResultDto<bool>.SuccessResult(true, "Stok baþarýyla güncellendi!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n? STOK HATA: {ex.Message}");
            return ResultDto<bool>.FailureResult($"Hata: {ex.Message}");
        }
    }
}