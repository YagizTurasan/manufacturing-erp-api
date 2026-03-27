using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.Interfaces;
using BahadirMakina.Domain.Entities;
using BahadirMakina.Domain.Enums;
using BahadirMakina.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BahadirMakina.Application.Services;

public class DepoService : IDepoService
{
    private readonly ApplicationDbContext _context;

    public DepoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResultDto<bool>> StokKontrolAsync(int depoId, int urunId, decimal gerekliMiktar)
    {
        try
        {
            var stok = await _context.Stoklar
                .FirstOrDefaultAsync(s => s.DepoId == depoId && s.UrunId == urunId);

            if (stok == null)
                return ResultDto<bool>.FailureResult($"Stok kaydý bulunamadý! (Depo: {depoId}, Ürün: {urunId})");

            if (stok.Miktar < gerekliMiktar)
                return ResultDto<bool>.FailureResult(
                    $"Yetersiz stok! Mevcut: {stok.Miktar}, Gerekli: {gerekliMiktar}"
                );

            return ResultDto<bool>.SuccessResult(true, "Stok yeterli");
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<bool>> RezervasyonYapAsync(
        int depoId,
        int urunId,
        decimal miktar,
        int isEmriId,
        int isAdimiId)
    {
        try
        {
            var stok = await _context.Stoklar
                .FirstOrDefaultAsync(s => s.DepoId == depoId && s.UrunId == urunId);

            if (stok == null)
                return ResultDto<bool>.FailureResult("Stok kaydý bulunamadý!");

            if (stok.Miktar < miktar)
                return ResultDto<bool>.FailureResult(
                    $"Yetersiz stok! Mevcut: {stok.Miktar}, Rezerve edilmek istenen: {miktar}"
                );
            stok.Durum = StokDurum.Rezerve;
            stok.IsEmriId = isEmriId;
            stok.IsAdimiId = isAdimiId;
            stok.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ResultDto<bool>.SuccessResult(true, $"{miktar} adet ürün rezerve edildi");
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<bool>> RezervasyonuIptalEtAsync(int depoId, int urunId, int isEmriId)
    {
        try
        {
            var stok = await _context.Stoklar
                .FirstOrDefaultAsync(s =>
                    s.DepoId == depoId &&
                    s.UrunId == urunId &&
                    s.IsEmriId == isEmriId);

            if (stok == null)
                return ResultDto<bool>.FailureResult("Rezerve edilmiþ stok bulunamadý!");

            stok.Durum = StokDurum.Hazir;
            stok.IsEmriId = null;
            stok.IsAdimiId = null;
            stok.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ResultDto<bool>.SuccessResult(true, "Rezervasyon iptal edildi");
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<int>> CreateDepoHareketAsync(
        int? isEmriId,
        int urunId,
        int? kaynakDepoId,
        int? hedefDepoId,
        decimal miktar,
        DepoHareketTip tip,
        int? istasyonId,
        int? isAdimiId,
        int? kullaniciId,
        string? aciklama)
    {
        try
        {
            var hareket = new DepoHareket
            {
                IsEmriId = isEmriId,
                UrunId = urunId,
                KaynakDepoId = kaynakDepoId,
                HedefDepoId = hedefDepoId,
                Miktar = miktar,
                Tip = tip,
                IstasyonId = istasyonId,
                IsAdimiId = isAdimiId,
                KullaniciId = kullaniciId,
                Aciklama = aciklama,
                IslemTarihi = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.DepoHareketleri.Add(hareket);
            await _context.SaveChangesAsync();

            return ResultDto<int>.SuccessResult(hareket.Id, "Depo hareketi kaydedildi");
        }
        catch (Exception ex)
        {
            return ResultDto<int>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<bool>> StokGuncelleAsync(
        int depoId,
        int urunId,
        decimal miktar,
        StokDurum durum)
    {
        try
        {
            var stok = await _context.Stoklar
                .FirstOrDefaultAsync(s => s.DepoId == depoId && s.UrunId == urunId);

            if (stok == null)
                return ResultDto<bool>.FailureResult("Stok kaydý bulunamadý!");

            stok.Miktar += miktar; // Pozitif ise artýr, negatif ise azalt

            if (stok.Miktar < 0)
                return ResultDto<bool>.FailureResult("Stok negatif olamaz!");

            stok.Durum = durum;
            stok.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ResultDto<bool>.SuccessResult(true, $"Stok güncellendi. Yeni miktar: {stok.Miktar}");
        }
        catch (Exception ex)
        {
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
            Console.WriteLine($"DepoId: {depoId}, UrunId: {urunId}, Miktar: {miktar}");
            Console.WriteLine($"Durum: {durum}, IsEmriId: {isEmriId}");
            if (durum == StokDurum.IslemBekliyor && isEmriId.HasValue)
            {
                Console.WriteLine("ÝþlemBekliyor durumu - Ýþ emri bazýnda kontrol yapýlýyor...");
                var isEmrineAitStok = await _context.Stoklar
                    .Where(s => s.DepoId == depoId &&
                               s.UrunId == urunId &&
                               s.Durum == StokDurum.IslemBekliyor &&
                               s.IsEmriId == isEmriId) // ? Ýþ Emri ID kontrolü!
                    .FirstOrDefaultAsync();

                if (isEmrineAitStok != null)
                {
                    Console.WriteLine($"? Ayný iþ emrine ait mevcut stok: ID={isEmrineAitStok.Id}, Miktar={isEmrineAitStok.Miktar}");
                    isEmrineAitStok.Miktar += miktar;
                    isEmrineAitStok.UpdatedAt = DateTime.UtcNow;
                    Console.WriteLine($"? Güncellendi: Yeni Miktar={isEmrineAitStok.Miktar}");
                }
                else
                {
                    Console.WriteLine("Yeni stok kaydý oluþturuluyor (Ýþ Emri bazýnda)...");
                    var yeniStok = new Stok
                    {
                        UrunId = urunId,
                        DepoId = depoId.Value,
                        Miktar = miktar,
                        Durum = durum,
                        IsEmriId = isEmriId, // ? Ýþ Emri ID'yi kaydet!
                        IsAdimiId = isAdimiId
                    };
                    await _context.Stoklar.AddAsync(yeniStok);
                    Console.WriteLine($"? Yeni stok oluþturuldu: Miktar={miktar}, IsEmriId={isEmriId}");
                }
            }
            else
            {
                Console.WriteLine("Hazýr durum - Genel stok kontrolü yapýlýyor...");

                var mevcutStok = await _context.Stoklar
                    .Where(s => s.DepoId == depoId &&
                               s.UrunId == urunId &&
                               s.Durum == durum)
                    .OrderByDescending(s => s.UpdatedAt)
                    .FirstOrDefaultAsync();

                if (mevcutStok != null)
                {
                    Console.WriteLine($"? Mevcut stok: ID={mevcutStok.Id}, Eski Miktar={mevcutStok.Miktar}");
                    mevcutStok.Miktar += miktar;
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