using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.DTOs.IsAdimi;
using BahadirMakina.Application.DTOs.IsEmri;
using BahadirMakina.Application.Interfaces;
using BahadirMakina.Domain.Entities;
using BahadirMakina.Domain.Enums;
using BahadirMakina.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BahadirMakina.Application.Services;

public class IsAdimiService : IIsAdimiService
{
    private readonly ApplicationDbContext _context;
    private readonly IDepoService _depoService;

    public IsAdimiService(ApplicationDbContext context, IDepoService depoService)
    {
        _context = context;
        _depoService = depoService;
    }

    public async Task<ResultDto<bool>> BaslatAsync(BaslatIsAdimiDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            Console.WriteLine($"=== ÝÞ BAÞLATMA DEBUG ===");
            Console.WriteLine($"IsAdimiId: {dto.IsAdimiId}");
            Console.WriteLine($"IstasyonId: {dto.IstasyonId}");
            Console.WriteLine($"KullaniciId: {dto.KullaniciId}");
            var isAdimi = await _context.IsAdimlari
                .Include(a => a.IsEmri)
                    .ThenInclude(i => i.Urun)
                .Include(a => a.Istasyon)
                .Include(a => a.GirdiDepo)
                .Include(a => a.CiktiDepo)
                .Include(a => a.GirdiUrun)
                .Include(a => a.GerekliBilesenler)
                    .ThenInclude(b => b.Urun)
                .FirstOrDefaultAsync(a => a.Id == dto.IsAdimiId);

            if (isAdimi == null)
            {
                Console.WriteLine("? Ýþ adýmý bulunamadý!");
                return ResultDto<bool>.FailureResult("Ýþ adýmý bulunamadý!");
            }

            Console.WriteLine($"Ýþ Adýmý Bulundu: {isAdimi.Id}");
            Console.WriteLine($"Ýþ Emri: {isAdimi.IsEmri.IsEmriNo}");
            Console.WriteLine($"Durum: {isAdimi.Durum}");
            Console.WriteLine($"GirdiDepoId: {isAdimi.GirdiDepoId}");
            Console.WriteLine($"CiktiDepoId: {isAdimi.CiktiDepoId}");
            Console.WriteLine($"GirdiUrunId: {isAdimi.GirdiUrunId}");
            Console.WriteLine($"Gerekli Bileþen Sayýsý: {isAdimi.GerekliBilesenler.Count}");
            if (isAdimi.Durum != IsAdimiDurum.Hazir && isAdimi.Durum != IsAdimiDurum.Beklemede)
            {
                Console.WriteLine($"? Durum uygun deðil: {isAdimi.Durum}");
                return ResultDto<bool>.FailureResult($"Ýþ adýmý baþlatýlamaz! Durum: {isAdimi.Durum}");
            }
            var istasyon = await _context.Istasyonlar.FindAsync(dto.IstasyonId);
            if (istasyon == null)
            {
                Console.WriteLine("? Ýstasyon bulunamadý!");
                return ResultDto<bool>.FailureResult("Ýstasyon bulunamadý!");
            }

            Console.WriteLine($"Ýstasyon: {istasyon.Ad}, Durum: {istasyon.Durum}");

            if (istasyon.Durum != IstasyonDurum.Musait)
            {
                Console.WriteLine($"? Ýstasyon müsait deðil!");
                return ResultDto<bool>.FailureResult($"Ýstasyon müsait deðil! Durum: {istasyon.Durum}");
            }
            var kullanici = await _context.Kullanicilar.FindAsync(dto.KullaniciId);
            if (kullanici == null)
            {
                Console.WriteLine("? Kullanýcý bulunamadý!");
                return ResultDto<bool>.FailureResult("Kullanýcý bulunamadý!");
            }

            Console.WriteLine($"Kullanýcý: {kullanici.KullaniciAdi}");
            Console.WriteLine("=== STOK KONTROLÜ ===");

            if (isAdimi.GerekliBilesenler.Any())
            {
                Console.WriteLine($"Çoklu bileþen modu - {isAdimi.GerekliBilesenler.Count} bileþen");

                foreach (var bilesen in isAdimi.GerekliBilesenler)
                {
                    Console.WriteLine($"Bileþen: {bilesen.Urun.Ad}, Miktar: {bilesen.Miktar}");

                    var stokKontrol = await _depoService.StokKontrolAsync(
                        isAdimi.GirdiDepoId!.Value,
                        bilesen.BilesenUrunId,
                        bilesen.Miktar
                    );

                    Console.WriteLine($"Stok Kontrol: Success={stokKontrol.Success}, Message={stokKontrol.Message}");

                    if (!stokKontrol.Success)
                    {
                        Console.WriteLine($"? YETERSÝZ STOK: {bilesen.Urun.Ad}");
                        return ResultDto<bool>.FailureResult(
                            $"{bilesen.Urun.Ad} için yetersiz stok! {stokKontrol.Message}"
                        );
                    }
                }
            }
            else if (isAdimi.GirdiUrunId.HasValue)
            {
                Console.WriteLine($"Tek ürün modu - {isAdimi.GirdiUrun!.Ad}");

                var stokKontrol = await _depoService.StokKontrolAsync(
                    isAdimi.GirdiDepoId!.Value,
                    isAdimi.GirdiUrunId.Value,
                    isAdimi.HedefMiktar
                );

                Console.WriteLine($"Stok Kontrol: Success={stokKontrol.Success}, Message={stokKontrol.Message}");

                if (!stokKontrol.Success)
                {
                    Console.WriteLine($"? YETERSÝZ STOK!");
                    return ResultDto<bool>.FailureResult(
                        $"{isAdimi.GirdiUrun!.Ad} için yetersiz stok! {stokKontrol.Message}"
                    );
                }
            }

            Console.WriteLine("? Stok kontrolü baþarýlý");
            Console.WriteLine("Ýþ adýmý güncelleniyor...");
            isAdimi.Durum = IsAdimiDurum.DevamEdiyor;
            isAdimi.BaslangicTarihi = DateTime.UtcNow;
            isAdimi.SorumluKullaniciId = dto.KullaniciId;
            isAdimi.UpdatedAt = DateTime.UtcNow;
            Console.WriteLine("Ýstasyon güncelleniyor...");
            istasyon.Durum = IstasyonDurum.Mesgul;
            istasyon.AktifIsAdimiId = isAdimi.Id;
            istasyon.UpdatedAt = DateTime.UtcNow;
            if (isAdimi.Sira == 1)
            {
                Console.WriteLine("Ýlk adým - Ýþ emri baþlatýlýyor...");
                isAdimi.IsEmri.Durum = IsEmriDurum.DevamEdiyor;
                isAdimi.IsEmri.BaslangicTarihi = DateTime.UtcNow;
                isAdimi.IsEmri.UpdatedAt = DateTime.UtcNow;
            }
            Console.WriteLine("Log kaydý oluþturuluyor...");
            var log = new Domain.Entities.IsAdimiLog
            {
                IsAdimiId = isAdimi.Id,
                KullaniciId = dto.KullaniciId,
                EskiDurum = IsAdimiDurum.Hazir,
                YeniDurum = IsAdimiDurum.DevamEdiyor,
                Aciklama = $"Ýþ adýmý baþlatýldý - Ýstasyon: {istasyon.Ad}",
                IslemTarihi = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            _context.IsAdimiLoglari.Add(log);
            Console.WriteLine("=== DEPO HAREKETLERÝ ===");

            if (isAdimi.GerekliBilesenler.Any())
            {
                Console.WriteLine($"Çoklu bileþen - {isAdimi.GerekliBilesenler.Count} bileþen iþlenecek");

                foreach (var bilesen in isAdimi.GerekliBilesenler)
                {
                    Console.WriteLine($"Bileþen iþleniyor: {bilesen.Urun.Ad} - {bilesen.Miktar} adet");

                    decimal toplamMiktar = bilesen.Miktar * isAdimi.HedefMiktar;
                    Console.WriteLine($"  1. Çýkýþ hareketi oluþturuluyor...");
                    await _depoService.CreateDepoHareketAsync(
                        isAdimi.IsEmriId,
                        bilesen.BilesenUrunId,
                        isAdimi.GirdiDepoId,
                        null,
                        toplamMiktar,
                        DepoHareketTip.Cikis,
                        istasyon.Id,
                        isAdimi.Id,
                        dto.KullaniciId,
                        $"{bilesen.Urun.Ad} iþleme gönderildi"
                    );
                    Console.WriteLine($"  2. Stok güncelleniyor...");
                    await _depoService.StokGuncelleAsync(
                        isAdimi.GirdiDepoId!.Value,
                        bilesen.BilesenUrunId,
                        -toplamMiktar,  
                        StokDurum.Hazir
                    );
                    var girdiStok = await _context.Stoklar
                        .FirstOrDefaultAsync(s => s.DepoId == isAdimi.GirdiDepoId &&
                                                 s.UrunId == bilesen.BilesenUrunId);

                    if (girdiStok != null && girdiStok.Miktar == 0)
                    {
                        Console.WriteLine($"  ?? Girdi deposunda stok 0 oldu, kayýt siliniyor: ID={girdiStok.Id}");
                        _context.Stoklar.Remove(girdiStok);
                    }
                    Console.WriteLine($"  3. Transfer hareketi oluþturuluyor...");
                    await _depoService.CreateDepoHareketAsync(
                        isAdimi.IsEmriId,
                        bilesen.BilesenUrunId,
                        isAdimi.GirdiDepoId,
                        isAdimi.CiktiDepoId,
                        toplamMiktar,
                        DepoHareketTip.Transfer,
                        istasyon.Id,
                        isAdimi.Id,
                        dto.KullaniciId,
                        $"{bilesen.Urun.Ad} {isAdimi.CiktiDepo.Ad}'ya gönderildi"
                    );
                    Console.WriteLine($"  4. Çýktý deposuna stok ekleniyor...");
                    await _depoService.StokOlusturVeyaGuncelleAsync(
                        isAdimi.CiktiDepoId,
                        bilesen.BilesenUrunId,
                        toplamMiktar,
                        StokDurum.IslemBekliyor,
                        isAdimi.IsEmriId,
                        isAdimi.Id
                    );

                    Console.WriteLine($"  ? {bilesen.Urun.Ad} iþlendi");
                }
            }
            else if (isAdimi.GirdiUrunId.HasValue)
            {
                Console.WriteLine($"Tek ürün modu - {isAdimi.GirdiUrun!.Ad}");
                Console.WriteLine("  1. Çýkýþ hareketi oluþturuluyor...");
                await _depoService.CreateDepoHareketAsync(
                    isAdimi.IsEmriId,
                    isAdimi.GirdiUrunId.Value,
                    isAdimi.GirdiDepoId,
                    null,
                    isAdimi.HedefMiktar,
                    DepoHareketTip.Cikis,
                    istasyon.Id,
                    isAdimi.Id,
                    dto.KullaniciId,
                    $"{isAdimi.GirdiUrun!.Ad} iþleme gönderildi"
                );
                Console.WriteLine("  2. Stok güncelleniyor...");
                await _depoService.StokGuncelleAsync(
                    isAdimi.GirdiDepoId!.Value,
                    isAdimi.GirdiUrunId.Value,
                    -isAdimi.HedefMiktar,
                    StokDurum.Hazir
                );
                var girdiStok = await _context.Stoklar
                    .FirstOrDefaultAsync(s => s.DepoId == isAdimi.GirdiDepoId &&
                                             s.UrunId == isAdimi.GirdiUrunId);

                if (girdiStok != null && girdiStok.Miktar == 0)
                {
                    Console.WriteLine($"  ?? Girdi deposunda stok 0 oldu, kayýt siliniyor: ID={girdiStok.Id}");
                    _context.Stoklar.Remove(girdiStok);
                }
                Console.WriteLine("  3. Transfer hareketi oluþturuluyor...");
                await _depoService.CreateDepoHareketAsync(
                    isAdimi.IsEmriId,
                    isAdimi.GirdiUrunId.Value,
                    isAdimi.GirdiDepoId,
                    isAdimi.CiktiDepoId,
                    isAdimi.HedefMiktar,
                    DepoHareketTip.Transfer,
                    istasyon.Id,
                    isAdimi.Id,
                    dto.KullaniciId,
                    $"{isAdimi.GirdiUrun.Ad} {isAdimi.CiktiDepo.Ad}'ya gönderildi"
                );
                Console.WriteLine("  4. Çýktý deposuna stok ekleniyor...");
                await _depoService.StokOlusturVeyaGuncelleAsync(
                    isAdimi.CiktiDepoId,
                    isAdimi.GirdiUrunId.Value,
                    isAdimi.HedefMiktar,
                    StokDurum.IslemBekliyor,
                    isAdimi.IsEmriId,
                    isAdimi.Id
                );

                Console.WriteLine("  ? Ýþlem tamamlandý");
            }

            Console.WriteLine("SaveChangesAsync çaðrýlýyor...");
            await _context.SaveChangesAsync();

            Console.WriteLine("Transaction commit ediliyor...");
            await transaction.CommitAsync();

            Console.WriteLine("? ÝÞ BAÞLATMA BAÞARILI!");
            Console.WriteLine("========================");

            return ResultDto<bool>.SuccessResult(true, "Ýþ adýmý baþarýyla baþlatýldý!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? EXCEPTION: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }

            await transaction.RollbackAsync();
            return ResultDto<bool>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<bool>> ParcaTamamlaAsync(ParcaTamamlaDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            Console.WriteLine($"\n=== PARÇA TAMAMLAMA ===");
            Console.WriteLine($"IsAdimiId: {dto.IsAdimiId}");
            Console.WriteLine($"Miktar: {dto.Miktar}");
            var isAdimi = await _context.IsAdimlari
                .Include(a => a.IsEmri)
                    .ThenInclude(i => i.Urun)
                .Include(a => a.Istasyon)
                .Include(a => a.CiktiDepo)
                .Include(a => a.GirdiUrun)
                .Include(a => a.CiktiUrun)
                .Include(a => a.GerekliBilesenler)
                    .ThenInclude(b => b.Urun)
                .FirstOrDefaultAsync(a => a.Id == dto.IsAdimiId);

            if (isAdimi == null)
            {
                Console.WriteLine("? Ýþ adýmý bulunamadý!");
                return ResultDto<bool>.FailureResult("Ýþ adýmý bulunamadý!");
            }

            Console.WriteLine($"Ýþ Emri: {isAdimi.IsEmri.IsEmriNo}");
            Console.WriteLine($"Adým Sýrasý: {isAdimi.Sira}");
            Console.WriteLine($"Mevcut Tamamlanan: {isAdimi.TamamlananMiktar}");
            Console.WriteLine($"Hedef Miktar: {isAdimi.HedefMiktar}");

            if (isAdimi.Durum != IsAdimiDurum.DevamEdiyor)
            {
                Console.WriteLine($"? Ýþ adýmý devam etmiyor! Durum: {isAdimi.Durum}");
                return ResultDto<bool>.FailureResult($"Ýþ adýmý devam etmiyor!");
            }
            isAdimi.TamamlananMiktar += dto.Miktar;
            isAdimi.UpdatedAt = DateTime.UtcNow;

            Console.WriteLine($"Yeni Tamamlanan Miktar: {isAdimi.TamamlananMiktar}");
            bool girdiCiktiAyni = isAdimi.GirdiUrunId.HasValue &&
                                  isAdimi.CiktiUrunId.HasValue &&
                                  isAdimi.GirdiUrunId == isAdimi.CiktiUrunId;

            Console.WriteLine($"Girdi = Çýktý: {girdiCiktiAyni}");

            if (girdiCiktiAyni)
            {
                Console.WriteLine("\n--- AYNI ÜRÜN MANTIK ---");
                var mevcutStok = await _context.Stoklar
                    .Where(s => s.DepoId == isAdimi.CiktiDepoId &&
                               s.UrunId == isAdimi.CiktiUrunId &&
                               s.IsEmriId == isAdimi.IsEmriId &&
                               s.Durum == StokDurum.IslemBekliyor)
                    .FirstOrDefaultAsync();

                if (mevcutStok != null)
                {
                    Console.WriteLine($"Mevcut stok: Miktar={mevcutStok.Miktar} (deðiþmedi)");
                    mevcutStok.UpdatedAt = DateTime.UtcNow;
                }

                if (isAdimi.GerekliBilesenler.Any())
                {
                    await BilesenTuket(isAdimi, dto.Miktar, dto.KullaniciId);
                }
            }
            else
            {
                Console.WriteLine("\n--- FARKLI ÜRÜN MANTIK ---");
                await _depoService.StokOlusturVeyaGuncelleAsync(
                    isAdimi.CiktiDepoId,
                    isAdimi.CiktiUrunId!.Value,
                    dto.Miktar,
                    StokDurum.IslemBekliyor,
                    isAdimi.IsEmriId,
                    isAdimi.Id
                );

                if (isAdimi.GerekliBilesenler.Any())
                {
                    await BilesenTuket(isAdimi, dto.Miktar, dto.KullaniciId);
                }
            }
            string mesaj = "Parça baþarýyla tamamlandý!";

            if (isAdimi.TamamlananMiktar >= isAdimi.HedefMiktar)
            {
                Console.WriteLine("\n? HEDEF MÝKTAR TAMAMLANDI!");
                Console.WriteLine("?? Durum DEÐÝÞMEDÝ - DevamEdiyor kalýyor");
                Console.WriteLine("?? Kullanýcý 'Ýþlemi Bitir' butonuna basmalý!");

                mesaj = $"Hedef miktar tamamlandý ({isAdimi.TamamlananMiktar}/{isAdimi.HedefMiktar})! Ýþlemi bitirmek için 'Ýþlemi Bitir' butonuna basýn.";
            }
            var log = new Domain.Entities.IsAdimiLog
            {
                IsAdimiId = isAdimi.Id,
                KullaniciId = dto.KullaniciId,
                EskiDurum = IsAdimiDurum.DevamEdiyor,
                YeniDurum = IsAdimiDurum.DevamEdiyor, // ? Deðiþmedi
                Aciklama = $"{dto.Miktar} adet tamamlandý",
                IslemTarihi = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            _context.IsAdimiLoglari.Add(log);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            Console.WriteLine("\n? PARÇA TAMAMLAMA BAÞARILI!");
            Console.WriteLine($"Mesaj: {mesaj}");
            Console.WriteLine("============================\n");

            return ResultDto<bool>.SuccessResult(true, mesaj);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"\n? HATA: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            return ResultDto<bool>.FailureResult($"Hata: {ex.Message}");
        }
    }
    private async Task BilesenTuket(IsAdimi isAdimi, decimal miktar, int kullaniciId)
    {
        Console.WriteLine($"\n--- Bileþenler Tüketiliyor ---");
        Console.WriteLine($"Tamamlanan Miktar: {miktar}");

        foreach (var bilesen in isAdimi.GerekliBilesenler)
        {
            decimal tuketilecekMiktar = bilesen.Miktar * miktar;

            Console.WriteLine($"Bileþen: {bilesen.Urun.Ad}");
            Console.WriteLine($"  Birim Miktar: {bilesen.Miktar}");
            Console.WriteLine($"  Tamamlanan: {miktar}");
            Console.WriteLine($"  Tüketilecek Toplam: {tuketilecekMiktar}");

            var bilesenStok = await _context.Stoklar
                .FirstOrDefaultAsync(s => s.DepoId == isAdimi.CiktiDepoId &&
                                         s.UrunId == bilesen.BilesenUrunId);

            if (bilesenStok != null)
            {
                Console.WriteLine($"  Mevcut Stok: {bilesenStok.Miktar}");

                bilesenStok.Miktar -= tuketilecekMiktar;
                bilesenStok.UpdatedAt = DateTime.UtcNow;

                Console.WriteLine($"  Yeni Stok: {bilesenStok.Miktar}");

                if (bilesenStok.Miktar <= 0)
                {
                    Console.WriteLine($"  ?? Bileþen stoku {bilesenStok.Miktar} oldu, kayýt siliniyor");
                    _context.Stoklar.Remove(bilesenStok);
                }

                var hareket = new DepoHareket
                {
                    IsEmriId = isAdimi.IsEmriId,
                    UrunId = bilesen.BilesenUrunId,
                    KaynakDepoId = isAdimi.CiktiDepoId,
                    HedefDepoId = null,
                    Miktar = tuketilecekMiktar,
                    Tip = DepoHareketTip.Cikis,
                    IstasyonId = isAdimi.IstasyonId,
                    IsAdimiId = isAdimi.Id,
                    KullaniciId = kullaniciId,
                    Aciklama = $"{bilesen.Urun.Ad} bileþen olarak tüketildi",
                    IslemTarihi = DateTime.UtcNow
                };
                await _context.DepoHareketleri.AddAsync(hareket);

                Console.WriteLine($"  ? {bilesen.Urun.Ad} tüketildi");
            }
            else
            {
                Console.WriteLine($"  ?? Bileþen stoku bulunamadý: {bilesen.Urun.Ad}");
            }
        }
    }

    public async Task<ResultDto<bool>> IslemiBitirAsync(int isAdimiId, int kullaniciId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            Console.WriteLine($"\n=== MANUEL ÝÞLEM BÝTÝRME ===");
            Console.WriteLine($"IsAdimiId: {isAdimiId}");
            var isAdimi = await _context.IsAdimlari
                .Include(a => a.Istasyon)
                .Include(a => a.IsEmri)
                .Include(a => a.CiktiDepo)
                .Include(a => a.CiktiUrun)
                .FirstOrDefaultAsync(a => a.Id == isAdimiId);

            if (isAdimi == null)
            {
                Console.WriteLine("? Ýþ adýmý bulunamadý!");
                return ResultDto<bool>.FailureResult("Ýþ adýmý bulunamadý!");
            }

            Console.WriteLine($"Ýþ Emri: {isAdimi.IsEmri.IsEmriNo}");
            Console.WriteLine($"Adým Sýrasý: {isAdimi.Sira}");
            Console.WriteLine($"Kalite Kontrol Gerekli: {isAdimi.KaliteKontrolGerekli}");
            Console.WriteLine($"TamamlananMiktar: {isAdimi.TamamlananMiktar}");
            Console.WriteLine($"HedefMiktar: {isAdimi.HedefMiktar}");
            if (isAdimi.Durum != IsAdimiDurum.DevamEdiyor)
            {
                Console.WriteLine($"? Ýþ adýmý devam etmiyor! Durum: {isAdimi.Durum}");
                return ResultDto<bool>.FailureResult($"Ýþ adýmý devam etmiyor! Durum: {isAdimi.Durum}");
            }
            if (isAdimi.TamamlananMiktar < isAdimi.HedefMiktar)
            {
                Console.WriteLine($"?? Hedef miktar tamamlanmadý! {isAdimi.TamamlananMiktar}/{isAdimi.HedefMiktar}");
                return ResultDto<bool>.FailureResult(
                    $"Hedef miktar tamamlanmadý! Tamamlanan: {isAdimi.TamamlananMiktar}/{isAdimi.HedefMiktar}"
                );
            }

            Console.WriteLine("? Hedef miktar kontrolü geçildi");
            if (isAdimi.KaliteKontrolGerekli)
            {
                Console.WriteLine("\n--- KALÝTE KONTROLE GÖNDERÝLÝYOR ---");
                isAdimi.Durum = IsAdimiDurum.KaliteKontrolde;
                isAdimi.BitisTarihi = DateTime.UtcNow;
                isAdimi.UpdatedAt = DateTime.UtcNow;

                Console.WriteLine($"? Ýþ adýmý durumu: {isAdimi.Durum}");
                Console.WriteLine("?? Stoklar ÝþlemBekliyor durumunda kalýyor");
                isAdimi.Istasyon.Durum = IstasyonDurum.Musait;
                isAdimi.Istasyon.AktifIsAdimiId = null;
                isAdimi.Istasyon.UpdatedAt = DateTime.UtcNow;
                Console.WriteLine($"? Ýstasyon serbest býrakýldý: {isAdimi.Istasyon.Ad}");
                var toplamAdimSayisi = await _context.IsAdimlari
                    .Where(a => a.IsEmriId == isAdimi.IsEmriId)
                    .CountAsync();

                Console.WriteLine($"Adým: {isAdimi.Sira}/{toplamAdimSayisi}");
                if (isAdimi.Sira == toplamAdimSayisi)
                {
                    Console.WriteLine("? SON ADIM - Ýþ Emri kalite kontrole gönderiliyor!");
                    isAdimi.IsEmri.Durum = IsEmriDurum.KaliteKontrolde;
                    isAdimi.IsEmri.UpdatedAt = DateTime.UtcNow;
                    Console.WriteLine("?? Ýþ Emri TamamlananMiktar güncellenmedi (Kalite kontrol bekleniyor)");
                }
                var kaliteLog = new Domain.Entities.IsAdimiLog
                {
                    IsAdimiId = isAdimi.Id,
                    KullaniciId = kullaniciId,
                    EskiDurum = IsAdimiDurum.DevamEdiyor,
                    YeniDurum = IsAdimiDurum.KaliteKontrolde,
                    Aciklama = "Ýþlem bitirildi - Kalite kontrole gönderildi",
                    IslemTarihi = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
                _context.IsAdimiLoglari.Add(kaliteLog);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                Console.WriteLine("\n? KALÝTE KONTROLE GÖNDERME BAÞARILI!");
                Console.WriteLine("==========================================\n");

                return ResultDto<bool>.SuccessResult(true, "Ýþlem tamamlandý! Kalite kontrole gönderildi.");
            }
            else
            {
                Console.WriteLine("\n--- KALÝTE KONTROL YOK - DÝREKT TAMAMLANIYOR ---");
                if (isAdimi.CiktiDepoId != 0 && isAdimi.CiktiUrunId.HasValue)
                {
                    Console.WriteLine("Çýktý stoklarý Hazýr duruma getiriliyor...");

                    var ciktiStok = await _context.Stoklar
                        .Where(s => s.DepoId == isAdimi.CiktiDepoId &&
                                   s.UrunId == isAdimi.CiktiUrunId &&
                                   s.IsEmriId == isAdimi.IsEmriId &&
                                   s.Durum == StokDurum.IslemBekliyor)
                        .FirstOrDefaultAsync();

                    if (ciktiStok != null)
                    {
                        Console.WriteLine($"Çýktý stok bulundu: Miktar={ciktiStok.Miktar}");
                        _context.Stoklar.Remove(ciktiStok);
                        Console.WriteLine("ÝþlemBekliyor stok silindi");
                        var hazirStok = await _context.Stoklar
                            .Where(s => s.DepoId == isAdimi.CiktiDepoId &&
                                       s.UrunId == isAdimi.CiktiUrunId &&
                                       s.Durum == StokDurum.Hazir)
                            .FirstOrDefaultAsync();

                        if (hazirStok != null)
                        {
                            hazirStok.Miktar += isAdimi.TamamlananMiktar;
                            hazirStok.UpdatedAt = DateTime.UtcNow;
                            Console.WriteLine($"Hazýr stok güncellendi: Yeni Miktar={hazirStok.Miktar}");
                        }
                        else
                        {
                            var yeniHazirStok = new Stok
                            {
                                UrunId = isAdimi.CiktiUrunId.Value,
                                DepoId = isAdimi.CiktiDepoId,
                                Miktar = isAdimi.TamamlananMiktar,
                                Durum = StokDurum.Hazir
                            };
                            await _context.Stoklar.AddAsync(yeniHazirStok);
                            Console.WriteLine($"Yeni Hazýr stok oluþturuldu: Miktar={isAdimi.TamamlananMiktar}");
                        }
                    }
                }
                var toplamAdimSayisi2 = await _context.IsAdimlari
                    .Where(a => a.IsEmriId == isAdimi.IsEmriId)
                    .CountAsync();

                Console.WriteLine($"Adým: {isAdimi.Sira}/{toplamAdimSayisi2}");

                if (isAdimi.Sira == toplamAdimSayisi2)
                {
                    Console.WriteLine("? SON ADIM - Ýþ Emri güncelleniyor!");
                    isAdimi.IsEmri.TamamlananMiktar = isAdimi.TamamlananMiktar;
                    isAdimi.IsEmri.UpdatedAt = DateTime.UtcNow;
                    Console.WriteLine($"Ýþ Emri: {isAdimi.IsEmri.TamamlananMiktar}/{isAdimi.IsEmri.HedefMiktar}");

                    if (isAdimi.IsEmri.TamamlananMiktar >= isAdimi.IsEmri.HedefMiktar)
                    {
                        Console.WriteLine("??? ÝÞ EMRÝ TAMAMLANDI!");
                        isAdimi.IsEmri.Durum = IsEmriDurum.Tamamlandi;
                        isAdimi.IsEmri.BitisTarihi = DateTime.UtcNow;
                    }
                }
                isAdimi.Durum = IsAdimiDurum.Tamamlandi;
                isAdimi.BitisTarihi = DateTime.UtcNow;
                isAdimi.UpdatedAt = DateTime.UtcNow;
                Console.WriteLine($"? Ýþ adýmý durumu: {isAdimi.Durum}");
                isAdimi.Istasyon.Durum = IstasyonDurum.Musait;
                isAdimi.Istasyon.AktifIsAdimiId = null;
                isAdimi.Istasyon.UpdatedAt = DateTime.UtcNow;
                Console.WriteLine($"? Ýstasyon serbest býrakýldý: {isAdimi.Istasyon.Ad}");
                await SonrakiAdimiHazirYapAsync(isAdimi.IsEmriId, isAdimi.Sira);
                var log = new Domain.Entities.IsAdimiLog
                {
                    IsAdimiId = isAdimi.Id,
                    KullaniciId = kullaniciId,
                    EskiDurum = IsAdimiDurum.DevamEdiyor,
                    YeniDurum = IsAdimiDurum.Tamamlandi,
                    Aciklama = "Ýþlem manuel olarak bitirildi",
                    IslemTarihi = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
                _context.IsAdimiLoglari.Add(log);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                Console.WriteLine("\n??? MANUEL ÝÞLEM BÝTÝRME BAÞARILI!");
                Console.WriteLine("=======================================\n");

                return ResultDto<bool>.SuccessResult(true, "Ýþlem baþarýyla bitirildi!");
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"\n? HATA: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            return ResultDto<bool>.FailureResult($"Hata: {ex.Message}");
        }
    }
    private async Task SonrakiAdimiHazirYapAsync(int isEmriId, int mevcutSira)
    {
        try
        {
            var sonrakiAdim = await _context.IsAdimlari
                .Where(ia => ia.IsEmriId == isEmriId &&
                             ia.Sira == mevcutSira + 1 &&
                             ia.Durum == IsAdimiDurum.Beklemede)
                .FirstOrDefaultAsync();

            if (sonrakiAdim != null)
            {
                sonrakiAdim.Durum = IsAdimiDurum.Hazir;
                sonrakiAdim.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                Console.WriteLine($"? Sonraki adým hazýr yapýldý: Sýra {sonrakiAdim.Sira}");
            }
            else
            {
                Console.WriteLine($"?? Sonraki adým bulunamadý (son adým olabilir)");
                await IsEmriTamamlandiMiKontrolAsync(isEmriId);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"?? Sonraki adým hazýr yapýlamadý: {ex.Message}");
        }
    }
    private async Task IsEmriTamamlandiMiKontrolAsync(int isEmriId)
    {
        try
        {
            var isEmri = await _context.IsEmirleri
                .Include(ie => ie.IsAdimlari)
                .FirstOrDefaultAsync(ie => ie.Id == isEmriId);

            if (isEmri == null) return;
            var tumAdimlarTamamlandi = isEmri.IsAdimlari.All(
                a => a.Durum == IsAdimiDurum.Tamamlandi
            );

            if (tumAdimlarTamamlandi)
            {
                Console.WriteLine("? TÜM ADIMLAR TAMAMLANDI - ÝÞ EMRÝ KAPATILIYOR");
                isEmri.Durum = IsEmriDurum.Tamamlandi;
                isEmri.BitisTarihi = DateTime.UtcNow;
                isEmri.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"?? Ýþ emri tamamlanma kontrolü hatasý: {ex.Message}");
        }
    }

    

    public async Task<ResultDto<List<BekleyenIsDto>>> GetBekleyenIslerByIstasyonAsync(int istasyonId)
    {
        try
        {
            var istasyon = await _context.Istasyonlar
                .FirstOrDefaultAsync(i => i.Id == istasyonId);

            if (istasyon == null)
                return ResultDto<List<BekleyenIsDto>>.FailureResult("Ýstasyon bulunamadý!");

            var bekleyenIsler = await _context.IsAdimlari
                .Include(a => a.IsEmri)
                    .ThenInclude(i => i.Urun)
                .Include(a => a.GerekliBilesenler)
                    .ThenInclude(b => b.Urun)
                .Include(a => a.GirdiDepo)
                .Where(a =>
                    a.IstasyonId == istasyonId &&
                    (a.Durum == IsAdimiDurum.Hazir || a.Durum == IsAdimiDurum.Beklemede))
                .OrderBy(a => a.IsEmri.CreatedAt)
                .ToListAsync();

            var dtoList = new List<BekleyenIsDto>();

            foreach (var is_adimi in bekleyenIsler)
            {
                var dto = new BekleyenIsDto
                {
                    IsAdimiId = is_adimi.Id,
                    IsEmriNo = is_adimi.IsEmri.IsEmriNo,
                    UrunAdi = is_adimi.IsEmri.Urun.Ad,
                    HedefMiktar = is_adimi.HedefMiktar,
                    Tanim = is_adimi.Tanim
                };
                if (is_adimi.GerekliBilesenler.Any())
                {
                    dto.GerekliBilesenler = new List<GerekliBilesenDto>();

                    foreach (var bilesen in is_adimi.GerekliBilesenler)
                    {
                        var stok = await _context.Stoklar
                            .FirstOrDefaultAsync(s =>
                                s.DepoId == is_adimi.GirdiDepoId &&
                                s.UrunId == bilesen.BilesenUrunId);

                        dto.GerekliBilesenler.Add(new GerekliBilesenDto
                        {
                            UrunAdi = bilesen.Urun.Ad,
                            Miktar = bilesen.Miktar,
                            StokYeterli = stok != null && stok.Miktar >= bilesen.Miktar
                        });
                    }
                }

                dtoList.Add(dto);
            }

            return ResultDto<List<BekleyenIsDto>>.SuccessResult(dtoList);
        }
        catch (Exception ex)
        {
            return ResultDto<List<BekleyenIsDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<AktifIsDto>> GetAktifIsByIstasyonAsync(int istasyonId)
    {
        try
        {
            var istasyon = await _context.Istasyonlar
                .Include(i => i.AktifIsAdimi)
                    .ThenInclude(a => a!.IsEmri)
                        .ThenInclude(e => e.Urun)
                .Include(i => i.AktifIsAdimi)
                    .ThenInclude(a => a!.SorumluKullanici)
                .FirstOrDefaultAsync(i => i.Id == istasyonId);

            if (istasyon == null)
                return ResultDto<AktifIsDto>.FailureResult("Ýstasyon bulunamadý!");

            if (istasyon.AktifIsAdimi == null)
                return ResultDto<AktifIsDto>.FailureResult("Ýstasyonda aktif iþ yok!");

            var aktifIs = istasyon.AktifIsAdimi;
            var gecenSure = DateTime.UtcNow - aktifIs.BaslangicTarihi!.Value;

            var dto = new AktifIsDto
            {
                IsAdimiId = aktifIs.Id,
                IsEmriNo = aktifIs.IsEmri.IsEmriNo,
                UrunAdi = aktifIs.IsEmri.Urun.Ad,
                Tanim = aktifIs.Tanim,
                HedefMiktar = aktifIs.HedefMiktar,
                TamamlananMiktar = aktifIs.TamamlananMiktar,
                BaslangicTarihi = aktifIs.BaslangicTarihi.Value,
                GecenSure = gecenSure,
                SorumluKullanici = aktifIs.SorumluKullanici?.AdSoyad ??
                                   aktifIs.SorumluKullanici?.KullaniciAdi ?? "Bilinmiyor"
            };

            return ResultDto<AktifIsDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            return ResultDto<AktifIsDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<BekleyenIsDto>>> GetBekleyenIslerByKullaniciAsync(int kullaniciId)
    {
        try
        {
            var kullanici = await _context.Kullanicilar.FindAsync(kullaniciId);
            if (kullanici == null)
                return ResultDto<List<BekleyenIsDto>>.FailureResult("Kullanýcý bulunamadý!");

            List<IsAdimi> bekleyenAdimlar;

            if (kullanici.Rol == KullaniciRol.KaliteKontrol)
            {
                bekleyenAdimlar = await _context.IsAdimlari
                    .Include(a => a.IsEmri)
                        .ThenInclude(e => e.Urun)
                    .Include(a => a.Istasyon)
                    .Where(a => a.Durum == IsAdimiDurum.KaliteKontrolde)
                    .OrderBy(a => a.CreatedAt)
                    .ToListAsync();
            }
            else if (kullanici.Rol == KullaniciRol.Kaynakci)
            {
                bekleyenAdimlar = await _context.IsAdimlari
                    .Include(a => a.IsEmri)
                        .ThenInclude(e => e.Urun)
                    .Include(a => a.Istasyon)
                    .Where(a => a.Durum == IsAdimiDurum.Hazir &&
                               (a.Istasyon.Tip == IstasyonTip.GazaltiKaynak ||
                                a.Istasyon.Tip == IstasyonTip.RobotKaynak))
                    .OrderBy(a => a.CreatedAt)
                    .ToListAsync();
            }
            else
            {
                IstasyonTip? istasyonTipi = kullanici.Rol switch
                {
                    KullaniciRol.Tornaci => IstasyonTip.Torna,
                    KullaniciRol.Abkantci => IstasyonTip.Abkant,
                    KullaniciRol.Lazerci => IstasyonTip.Lazer,
                    KullaniciRol.Kumlamaci => IstasyonTip.Kumlama,
                    KullaniciRol.Taslamaci => IstasyonTip.Taslama,
                    KullaniciRol.Kaynakci => IstasyonTip.GazaltiKaynak | IstasyonTip.RobotKaynak,
                    KullaniciRol.DogrultmaOperatoru => IstasyonTip.Dogrultma,
                    KullaniciRol.Presci => IstasyonTip.HidrolikPres,
                    KullaniciRol.DikIslemci => IstasyonTip.DikIslem,
                    _ => null
                };

                if (istasyonTipi == null)
                    return ResultDto<List<BekleyenIsDto>>.SuccessResult(new List<BekleyenIsDto>());
                bekleyenAdimlar = await _context.IsAdimlari
                    .Include(a => a.IsEmri)
                        .ThenInclude(e => e.Urun)
                    .Include(a => a.Istasyon)
                    .Where(a => a.Durum == IsAdimiDurum.Hazir &&
                               a.Istasyon.Tip == istasyonTipi.Value)
                    .OrderBy(a => a.CreatedAt)
                    .ToListAsync();
            }

            var bekleyenIsler = bekleyenAdimlar.Select(a => new BekleyenIsDto
            {
                IsAdimiId = a.Id,
                IsEmriNo = a.IsEmri.IsEmriNo,
                UrunAdi = a.IsEmri.Urun.Ad,
                UrunKodu = a.IsEmri.Urun.Kod,
                HedefMiktar = a.HedefMiktar,
                Sira = a.Sira,
                IstasyonId = a.IstasyonId, // ? BU SATIRDA istasyonId SET EDÝLÝYOR
                IstasyonTipi = a.Istasyon.Tip.ToString(),
                Tanim = a.Tanim,
                Oncelik = "Orta",
                CreatedAt = a.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                GerekliBilesenler = null
            }).ToList();

            return ResultDto<List<BekleyenIsDto>>.SuccessResult(bekleyenIsler);
        }
        catch (Exception ex)
        {
            return ResultDto<List<BekleyenIsDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<AktifIsDetayDto>> GetAktifIsByKullaniciAsync(int kullaniciId)
    {
        try
        {
            var aktifAdim = await _context.IsAdimlari
                .Include(a => a.IsEmri)
                    .ThenInclude(e => e.Urun)
                .Include(a => a.Istasyon)
                .Include(a => a.SorumluKullanici)
                .FirstOrDefaultAsync(a =>
                    a.SorumluKullaniciId == kullaniciId &&
                    a.Durum == IsAdimiDurum.DevamEdiyor);

            if (aktifAdim == null)
                return ResultDto<AktifIsDetayDto>.FailureResult("Aktif iþ bulunamadý!");
            var hurdaMiktari = await _context.Hurdalar
                .Where(h => h.IsAdimiId == aktifAdim.Id)
                .SumAsync(h => h.Miktar);
            var gecenSure = aktifAdim.BaslangicTarihi.HasValue
                ? (int)(DateTime.UtcNow - aktifAdim.BaslangicTarihi.Value).TotalMinutes
                : 0;

            var detay = new AktifIsDetayDto
            {
                IsAdimiId = aktifAdim.Id,
                IsEmriNo = aktifAdim.IsEmri.IsEmriNo,
                UrunAdi = aktifAdim.IsEmri.Urun.Ad,
                Sira = aktifAdim.Sira,
                IstasyonAdi = aktifAdim.Istasyon.Ad,
                IstasyonTipi = aktifAdim.Istasyon.Tip.ToString(),
                Tanim = aktifAdim.Tanim,
                HedefMiktar = aktifAdim.HedefMiktar,
                TamamlananMiktar = aktifAdim.TamamlananMiktar,
                KumulatifTamamlanan = aktifAdim.TamamlananMiktar,
                KalanMiktar = aktifAdim.HedefMiktar - aktifAdim.TamamlananMiktar,
                HurdaMiktari = hurdaMiktari,
                Durum = aktifAdim.Durum.ToString(),
                BaslangisTarihi = aktifAdim.BaslangicTarihi!.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                GecenSure = gecenSure,
                KaliteKontrolGerekli = aktifAdim.KaliteKontrolGerekli,
                SorumluKullaniciId = aktifAdim.SorumluKullaniciId,
                SorumluKullaniciAdi = aktifAdim.SorumluKullanici?.AdSoyad ?? aktifAdim.SorumluKullanici?.KullaniciAdi
            };

            return ResultDto<AktifIsDetayDto>.SuccessResult(detay);
        }
        catch (Exception ex)
        {
            return ResultDto<AktifIsDetayDto>.FailureResult($"Hata: {ex.Message}");
        }
    }
}