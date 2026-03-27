using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.DTOs.KaliteKontrol;
using BahadirMakina.Application.Interfaces;
using BahadirMakina.Domain.Entities;
using BahadirMakina.Domain.Enums;
using BahadirMakina.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BahadirMakina.Application.Services;

public class KaliteKontrolService : IKaliteKontrolService
{
    private readonly ApplicationDbContext _context;
    private readonly IDepoService _depoService;

    public KaliteKontrolService(ApplicationDbContext context, IDepoService depoService)
    {
        _context = context;
        _depoService = depoService;
    }

    public async Task<ResultDto<int>> CreateKaliteKontrolAsync(CreateKaliteKontrolDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            Console.WriteLine($"\n=== KALÝTE KONTROL OLUÞTURMA ===");
            Console.WriteLine($"IsAdimiId: {dto.IsAdimiId}");
            Console.WriteLine($"Kontrol Edilen: {dto.KontrolEdilenMiktar}");
            Console.WriteLine($"Onaylanan: {dto.OnaylananMiktar}");
            Console.WriteLine($"Red: {dto.RedMiktar}");
            var isAdimi = await _context.IsAdimlari
                .Include(a => a.IsEmri)
                    .ThenInclude(e => e.Urun)
                .Include(a => a.CiktiDepo)
                .Include(a => a.CiktiUrun)
                .Include(a => a.GerekliBilesenler)
                    .ThenInclude(b => b.Urun)
                .FirstOrDefaultAsync(a => a.Id == dto.IsAdimiId);

            if (isAdimi == null)
            {
                Console.WriteLine("? Ýþ adýmý bulunamadý!");
                return ResultDto<int>.FailureResult("Ýþ adýmý bulunamadý!");
            }

            Console.WriteLine($"Ýþ Emri: {isAdimi.IsEmri.IsEmriNo}");
            Console.WriteLine($"Adým Sýrasý: {isAdimi.Sira}");
            Console.WriteLine($"Durum: {isAdimi.Durum}");

            if (isAdimi.Durum != IsAdimiDurum.KaliteKontrolde)
            {
                Console.WriteLine($"? Ýþ adýmý kalite kontrolde deðil!");
                return ResultDto<int>.FailureResult(
                    $"Ýþ adýmý kalite kontrolde deðil! Durum: {isAdimi.Durum}"
                );
            }
            if (dto.OnaylananMiktar + dto.RedMiktar != dto.KontrolEdilenMiktar)
            {
                Console.WriteLine("? Onaylanan + Red ? Kontrol Edilen!");
                return ResultDto<int>.FailureResult(
                    "Onaylanan + Red = Kontrol Edilen olmalýdýr!"
                );
            }

            if (dto.KontrolEdilenMiktar > isAdimi.TamamlananMiktar)
            {
                Console.WriteLine($"? Kontrol edilen miktar fazla! {dto.KontrolEdilenMiktar} > {isAdimi.TamamlananMiktar}");
                return ResultDto<int>.FailureResult(
                    $"Kontrol edilen miktar, tamamlanan miktardan fazla olamaz! " +
                    $"(Tamamlanan: {isAdimi.TamamlananMiktar})"
                );
            }

            Console.WriteLine("? Miktar kontrolleri geçildi");
            var sonuc = dto.RedMiktar > 0
                ? KaliteKontrolSonuc.Reddedildi
                : KaliteKontrolSonuc.Onaylandi;

            Console.WriteLine($"Sonuç: {sonuc}");
            var kaliteKontrol = new KaliteKontrol
            {
                IsAdimiId = dto.IsAdimiId,
                UrunId = isAdimi.CiktiUrunId!.Value,
                DepoId = isAdimi.CiktiDepoId,
                KontrolEdilenMiktar = dto.KontrolEdilenMiktar,
                OnaylananMiktar = dto.OnaylananMiktar,
                RedMiktar = dto.RedMiktar,
                Sonuc = sonuc,
                RedSebebi = dto.RedSebebi,
                KontrolEdenKullaniciId = dto.KontrolEdenKullaniciId,
                Notlar = dto.Notlar,
                KontrolTarihi = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.KaliteKontroller.Add(kaliteKontrol);
            await _context.SaveChangesAsync();
            Console.WriteLine($"? Kalite kontrol kaydý oluþturuldu: ID={kaliteKontrol.Id}");
            Console.WriteLine("\n--- ÝþlemBekliyor Stok Ýþlemi ---");

            var islemBekliyorStok = await _context.Stoklar
                .Where(s => s.DepoId == isAdimi.CiktiDepoId &&
                           s.UrunId == isAdimi.CiktiUrunId &&
                           s.IsEmriId == isAdimi.IsEmriId &&
                           s.Durum == StokDurum.IslemBekliyor)
                .FirstOrDefaultAsync();

            if (islemBekliyorStok == null)
            {
                Console.WriteLine("? ÝþlemBekliyor stok bulunamadý!");
                return ResultDto<int>.FailureResult("ÝþlemBekliyor stok bulunamadý!");
            }

            Console.WriteLine($"ÝþlemBekliyor Stok: ID={islemBekliyorStok.Id}, Miktar={islemBekliyorStok.Miktar}");
            if (dto.OnaylananMiktar > 0)
            {
                Console.WriteLine($"\n--- Onaylanan Ürünler Ýþleniyor ({dto.OnaylananMiktar} adet) ---");
                await _depoService.CreateDepoHareketAsync(
                    isAdimi.IsEmriId,
                    isAdimi.CiktiUrunId.Value,
                    isAdimi.CiktiDepoId,
                    null,
                    dto.OnaylananMiktar,
                    DepoHareketTip.Cikis,
                    isAdimi.IstasyonId,
                    isAdimi.Id,
                    dto.KontrolEdenKullaniciId,
                    "Kalite kontrol OK - Çýkýþ"
                );
                Console.WriteLine("? Çýkýþ hareketi kaydedildi");
                var hedefDepo = await BelirleHedefDepoAsync(isAdimi);
                Console.WriteLine($"Hedef Depo: {hedefDepo.Ad}");

                await _depoService.CreateDepoHareketAsync(
                    isAdimi.IsEmriId,
                    isAdimi.CiktiUrunId.Value,
                    isAdimi.CiktiDepoId,
                    hedefDepo.Id,
                    dto.OnaylananMiktar,
                    DepoHareketTip.Transfer,
                    null,
                    isAdimi.Id,
                    dto.KontrolEdenKullaniciId,
                    $"Kalite kontrol OK - {hedefDepo.Ad}'ya transfer"
                );
                Console.WriteLine("? Transfer hareketi kaydedildi");
                await _depoService.StokOlusturVeyaGuncelleAsync(
                    hedefDepo.Id,
                    isAdimi.CiktiUrunId.Value,
                    dto.OnaylananMiktar,
                    StokDurum.Hazir
                );
                Console.WriteLine($"? Hedef depoya {dto.OnaylananMiktar} adet Hazýr stok eklendi");
            }
            if (dto.RedMiktar > 0)
            {
                Console.WriteLine($"\n--- Reddedilen Ürünler Ýþleniyor ({dto.RedMiktar} adet) ---");
                var hurda = new Hurda
                {
                    UrunId = isAdimi.CiktiUrunId!.Value,
                    Miktar = dto.RedMiktar,
                    Sebep = dto.RedSebebi ?? "Kalite kontrol reddedildi",
                    IsEmriId = isAdimi.IsEmriId,
                    IsAdimiId = isAdimi.Id,
                    KaliteKontrolId = kaliteKontrol.Id,
                    KaynakDepoId = isAdimi.CiktiDepoId,
                    TahminiMaliyetKaybi = await HesaplaMaliyetKaybiAsync(isAdimi, dto.RedMiktar),
                    HurdaTarihi = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Hurdalar.Add(hurda);
                Console.WriteLine("? Hurda kaydý oluþturuldu");
                await _depoService.CreateDepoHareketAsync(
                    isAdimi.IsEmriId,
                    isAdimi.CiktiUrunId.Value,
                    isAdimi.CiktiDepoId,
                    null,
                    dto.RedMiktar,
                    DepoHareketTip.Hurda,
                    isAdimi.IstasyonId,
                    isAdimi.Id,
                    dto.KontrolEdenKullaniciId,
                    $"Kalite kontrol RED - Hurdaya: {dto.RedSebebi}"
                );
                Console.WriteLine("? Hurda çýkýþ hareketi kaydedildi");
                var hurdaDepo = await _context.Depolar
                    .FirstOrDefaultAsync(d => d.Tip == DepoTip.Hurda);

                if (hurdaDepo != null)
                {
                    Console.WriteLine($"Hurda Deposu: {hurdaDepo.Ad}");

                    await _depoService.CreateDepoHareketAsync(
                        isAdimi.IsEmriId,
                        isAdimi.CiktiUrunId.Value,
                        isAdimi.CiktiDepoId,
                        hurdaDepo.Id,
                        dto.RedMiktar,
                        DepoHareketTip.Transfer,
                        null,
                        isAdimi.Id,
                        dto.KontrolEdenKullaniciId,
                        "Hurda deposuna aktarýldý"
                    );

                    await _depoService.StokOlusturVeyaGuncelleAsync(
                        hurdaDepo.Id,
                        isAdimi.CiktiUrunId.Value,
                        dto.RedMiktar,
                        StokDurum.Hurda
                    );
                    Console.WriteLine($"? Hurda deposuna {dto.RedMiktar} adet eklendi");
                }
                else
                {
                    Console.WriteLine("?? Hurda deposu bulunamadý!");
                }
            }
            Console.WriteLine("\n--- ÝþlemBekliyor Stok Siliniyor ---");
            Console.WriteLine($"Silinen: ID={islemBekliyorStok.Id}, Miktar={islemBekliyorStok.Miktar}");
            _context.Stoklar.Remove(islemBekliyorStok);
            Console.WriteLine("? ÝþlemBekliyor stok silindi");
            Console.WriteLine("\n--- Ýþ Adýmý Güncelleniyor ---");
            isAdimi.Durum = IsAdimiDurum.Tamamlandi;
            isAdimi.KaliteKontrolTarihi = DateTime.UtcNow;
            isAdimi.UpdatedAt = DateTime.UtcNow;
            Console.WriteLine($"? Ýþ adýmý durumu: {isAdimi.Durum}");
            Console.WriteLine("\n--- Ýþ Emri Güncelleme ---");

            var toplamAdimSayisi = await _context.IsAdimlari
                .Where(a => a.IsEmriId == isAdimi.IsEmriId)
                .CountAsync();

            Console.WriteLine($"Adým: {isAdimi.Sira}/{toplamAdimSayisi}");
            if (isAdimi.Sira == toplamAdimSayisi)
            {
                Console.WriteLine("? SON ADIM - Ýþ Emri güncelleniyor!");
                isAdimi.IsEmri.TamamlananMiktar = dto.OnaylananMiktar;
                isAdimi.IsEmri.HurdaMiktar = dto.RedMiktar;
                isAdimi.IsEmri.UpdatedAt = DateTime.UtcNow;

                Console.WriteLine($"Ýþ Emri Tamamlanan: {isAdimi.IsEmri.TamamlananMiktar}/{isAdimi.IsEmri.HedefMiktar}");
                Console.WriteLine($"Ýþ Emri Hurda: {isAdimi.IsEmri.HurdaMiktar}");

                isAdimi.IsEmri.Durum = IsEmriDurum.Tamamlandi;
                isAdimi.IsEmri.BitisTarihi = DateTime.UtcNow;
                if (isAdimi.IsEmri.TamamlananMiktar >= isAdimi.IsEmri.HedefMiktar)
                {
                    Console.WriteLine("??? ÝÞ EMRÝ TAMAMLANDI!");
                    isAdimi.IsEmri.Durum = IsEmriDurum.Tamamlandi;
                    isAdimi.IsEmri.BitisTarihi = DateTime.UtcNow;
                }
            }
            else
            {
                Console.WriteLine($"?? Ara adým - Ýþ Emri güncellenmedi");
            }
            var log = new IsAdimiLog
            {
                IsAdimiId = isAdimi.Id,
                KullaniciId = dto.KontrolEdenKullaniciId,
                EskiDurum = IsAdimiDurum.KaliteKontrolde,
                YeniDurum = IsAdimiDurum.Tamamlandi,
                Aciklama = $"Kalite kontrol tamamlandý - Onay: {dto.OnaylananMiktar}, Red: {dto.RedMiktar}",
                IslemTarihi = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            _context.IsAdimiLoglari.Add(log);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            Console.WriteLine("\n??? KALÝTE KONTROL BAÞARILI!");
            Console.WriteLine("================================\n");

            return ResultDto<int>.SuccessResult(
                kaliteKontrol.Id,
                $"Kalite kontrol tamamlandý! Onay: {dto.OnaylananMiktar}, Red: {dto.RedMiktar}"
            );
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"\n? HATA: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            return ResultDto<int>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<BekleyenKaliteKontrolDto>>> GetBekleyenKontrollerAsync()
    {
        try
        {
            var bekleyenler = await _context.IsAdimlari
                .Include(a => a.IsEmri)
                    .ThenInclude(e => e.Urun)
                .Include(a => a.CiktiDepo)
                .Include(a => a.Istasyon)
                .Include(a => a.SorumluKullanici)
                .Where(a => a.Durum == IsAdimiDurum.KaliteKontrolde)
                .OrderBy(a => a.BitisTarihi)
                .ToListAsync();

            var dtoList = bekleyenler.Select(a => new BekleyenKaliteKontrolDto
            {
                IsAdimiId = a.Id,
                IsEmriNo = a.IsEmri.IsEmriNo,
                UrunAdi = a.IsEmri.Urun.Ad,
                DepoAdi = a.CiktiDepo.Ad,
                Miktar = a.TamamlananMiktar,
                IstasyonAdi = a.Istasyon.Ad,
                SorumluOperator = a.SorumluKullanici?.AdSoyad ??
                                  a.SorumluKullanici?.KullaniciAdi ?? "Bilinmiyor",
                BitisTarihi = a.BitisTarihi!.Value
            }).ToList();

            return ResultDto<List<BekleyenKaliteKontrolDto>>.SuccessResult(dtoList);
        }
        catch (Exception ex)
        {
            return ResultDto<List<BekleyenKaliteKontrolDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<KaliteKontrolDto>> GetByIdAsync(int id)
    {
        try
        {
            var kaliteKontrol = await _context.KaliteKontroller
                .Include(k => k.IsAdimi)
                    .ThenInclude(a => a.IsEmri)
                .Include(k => k.Urun)
                .Include(k => k.Depo)
                .Include(k => k.KontrolEdenKullanici)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kaliteKontrol == null)
                return ResultDto<KaliteKontrolDto>.FailureResult("Kalite kontrol kaydý bulunamadý!");

            var dto = new KaliteKontrolDto
            {
                Id = kaliteKontrol.Id,
                IsAdimiId = kaliteKontrol.IsAdimiId,
                IsEmriNo = kaliteKontrol.IsAdimi.IsEmri.IsEmriNo,
                UrunAdi = kaliteKontrol.Urun.Ad,
                DepoAdi = kaliteKontrol.Depo.Ad,
                KontrolEdilenMiktar = kaliteKontrol.KontrolEdilenMiktar,
                OnaylananMiktar = kaliteKontrol.OnaylananMiktar,
                RedMiktar = kaliteKontrol.RedMiktar,
                Sonuc = kaliteKontrol.Sonuc.ToString(),
                RedSebebi = kaliteKontrol.RedSebebi,
                KontrolEdenKullanici = kaliteKontrol.KontrolEdenKullanici.AdSoyad ??
                                       kaliteKontrol.KontrolEdenKullanici.KullaniciAdi,
                KontrolTarihi = kaliteKontrol.KontrolTarihi
            };

            return ResultDto<KaliteKontrolDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            return ResultDto<KaliteKontrolDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<KaliteKontrolDto>> GetByIsAdimiIdAsync(int isAdimiId)
    {
        try
        {
            var kaliteKontrol = await _context.KaliteKontroller
                .Include(k => k.IsAdimi)
                    .ThenInclude(a => a.IsEmri)
                .Include(k => k.Urun)
                .Include(k => k.Depo)
                .Include(k => k.KontrolEdenKullanici)
                .FirstOrDefaultAsync(k => k.IsAdimiId == isAdimiId);

            if (kaliteKontrol == null)
                return ResultDto<KaliteKontrolDto>.FailureResult("Bu iþ adýmý için kalite kontrol kaydý bulunamadý!");

            var dto = new KaliteKontrolDto
            {
                Id = kaliteKontrol.Id,
                IsAdimiId = kaliteKontrol.IsAdimiId,
                IsEmriNo = kaliteKontrol.IsAdimi.IsEmri.IsEmriNo,
                UrunAdi = kaliteKontrol.Urun.Ad,
                DepoAdi = kaliteKontrol.Depo.Ad,
                KontrolEdilenMiktar = kaliteKontrol.KontrolEdilenMiktar,
                OnaylananMiktar = kaliteKontrol.OnaylananMiktar,
                RedMiktar = kaliteKontrol.RedMiktar,
                Sonuc = kaliteKontrol.Sonuc.ToString(),
                RedSebebi = kaliteKontrol.RedSebebi,
                KontrolEdenKullanici = kaliteKontrol.KontrolEdenKullanici.AdSoyad ??
                                       kaliteKontrol.KontrolEdenKullanici.KullaniciAdi,
                KontrolTarihi = kaliteKontrol.KontrolTarihi
            };

            return ResultDto<KaliteKontrolDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            return ResultDto<KaliteKontrolDto>.FailureResult($"Hata: {ex.Message}");
        }
    }
    private async Task<Depo> BelirleHedefDepoAsync(IsAdimi isAdimi)
    {
        var urun = await _context.Urunler.FindAsync(isAdimi.CiktiUrunId);

        if (urun!.Tip == UrunTip.Mamul)
        {
            return await _context.Depolar
                .FirstAsync(d => d.Tip == DepoTip.Sevk);
        }
        else
        {
            return await _context.Depolar
                .FirstAsync(d => d.Tip == DepoTip.YariMamul);
        }
    }

    private async Task<decimal?> HesaplaMaliyetKaybiAsync(IsAdimi isAdimi, decimal hurdaMiktar)
    {
        var urun = await _context.Urunler.FindAsync(isAdimi.CiktiUrunId);

        if (urun == null) return null;

        decimal birimMaliyet = urun.Tip switch
        {
            UrunTip.Hammadde => 50m,
            UrunTip.YariMamul => 120m,
            UrunTip.Mamul => 500m,
            _ => 0m
        };

        return birimMaliyet * hurdaMiktar;
    }
}