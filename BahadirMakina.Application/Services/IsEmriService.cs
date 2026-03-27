using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.DTOs.IsEmri;
using BahadirMakina.Application.Interfaces;
using BahadirMakina.Domain.Entities;
using BahadirMakina.Domain.Enums;
using BahadirMakina.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BahadirMakina.Application.Services;

public class IsEmriService : IIsEmriService
{
    private readonly ApplicationDbContext _context;
    private readonly IDepoService _depoService;

    public IsEmriService(ApplicationDbContext context, IDepoService depoService)
    {
        _context = context;
        _depoService = depoService;
    }

    public async Task<ResultDto<IsEmriDto>> CreateIsEmriAsync(CreateIsEmriDto dto, int olusturanKullaniciId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            Console.WriteLine($"\n=== ÝÞ EMRÝ OLUÞTURMA BAÞLADI ===");
            Console.WriteLine($"UrunId: {dto.UrunId}");
            Console.WriteLine($"HedefMiktar: {dto.HedefMiktar}");
            var urun = await _context.Urunler
                .Include(u => u.UrunAgaciAdimlari)
                    .ThenInclude(a => a.HedefDepo)
                .Include(u => u.GerekliOlduguUrunler)
                .FirstOrDefaultAsync(u => u.Id == dto.UrunId);

            if (urun == null)
            {
                Console.WriteLine("? Ürün bulunamadý!");
                await transaction.RollbackAsync();
                return ResultDto<IsEmriDto>.FailureResult("Ürün bulunamadý!");
            }

            Console.WriteLine($"? Ürün bulundu: {urun.Ad} ({urun.Kod})");
            Console.WriteLine($"Ürün Tipi: {urun.Tip}");
            if (urun.Tip == UrunTip.Hammadde)
            {
                Console.WriteLine("?? Hammadde ürünler için iþ emri oluþturulamaz!");
                await transaction.RollbackAsync();
                return ResultDto<IsEmriDto>.FailureResult("Hammadde ürünler için iþ emri oluþturulamaz! Direkt stok giriþi yapýnýz.");
            }
            var urunAgaciAdimlari = urun.UrunAgaciAdimlari
                .OrderBy(a => a.Sira)
                .ToList();

            if (!urunAgaciAdimlari.Any())
            {
                Console.WriteLine("? Bu ürün için üretim adýmý tanýmlanmamýþ!");
                await transaction.RollbackAsync();
                return ResultDto<IsEmriDto>.FailureResult(
                    "Bu ürün için üretim adýmý tanýmlanmamýþ! Lütfen önce ürün aðacýný oluþturun.");
            }

            Console.WriteLine($"? Ürün aðacý bulundu: {urunAgaciAdimlari.Count} adým");
            foreach (var adim in urunAgaciAdimlari)
            {
                if (adim.HedefDepoId == 0 || adim.HedefDepo == null)
                {
                    Console.WriteLine($"? Adým {adim.Sira} ({adim.IstasyonTipi}) için hedef depo tanýmlý deðil!");
                    await transaction.RollbackAsync();
                    return ResultDto<IsEmriDto>.FailureResult(
                        $"Adým {adim.Sira} ({adim.IstasyonTipi}) için hedef depo tanýmlanmamýþ! Lütfen ürün aðacýný kontrol edin.");
                }
                Console.WriteLine($"  Adým {adim.Sira}: {adim.IstasyonTipi} › {adim.HedefDepo.Ad} ?");
            }
            var gerekliBilesenler = urun.GerekliOlduguUrunler.ToList();
            Console.WriteLine($"\nGerekli Bileþen Sayýsý: {gerekliBilesenler.Count}");

            if (gerekliBilesenler.Any())
            {
                foreach (var bilesen in gerekliBilesenler)
                {
                    var bilesenUrun = await _context.Urunler.FindAsync(bilesen.BilesenUrunId);
                    Console.WriteLine($"  - {bilesenUrun?.Ad}: {bilesen.Miktar} adet (Birim baþýna)");
                }
            }
            var hammaddeDepo = await _context.Depolar
                .FirstOrDefaultAsync(d => d.Tip == DepoTip.Hammadde);

            if (hammaddeDepo == null)
            {
                Console.WriteLine("? Hammadde deposu bulunamadý!");
                await transaction.RollbackAsync();
                return ResultDto<IsEmriDto>.FailureResult("Hammadde deposu tanýmlý deðil!");
            }

            Console.WriteLine($"? Hammadde Deposu: {hammaddeDepo.Ad} (ID: {hammaddeDepo.Id})");
            Console.WriteLine("\n=== STOK KONTROLÜ ===");

            if (gerekliBilesenler.Any())
            {
                var ilkBilesen = gerekliBilesenler.First();
                var ilkBilesenUrun = await _context.Urunler.FindAsync(ilkBilesen.BilesenUrunId);

                DepoTip girdiDepoTipi = ilkBilesenUrun?.Tip == UrunTip.YariMamul
                    ? DepoTip.YariMamul
                    : DepoTip.Hammadde;

                var girdiDepo = await _context.Depolar
                    .FirstOrDefaultAsync(d => d.Tip == girdiDepoTipi);

                if (girdiDepo == null)
                {
                    Console.WriteLine($"? {girdiDepoTipi} deposu bulunamadý!");
                    await transaction.RollbackAsync();
                    return ResultDto<IsEmriDto>.FailureResult($"{girdiDepoTipi} deposu tanýmlý deðil!");
                }

                Console.WriteLine($"Girdi Deposu: {girdiDepo.Ad} (Tip: {girdiDepo.Tip})");
                Console.WriteLine($"Hedef Miktar: {dto.HedefMiktar}");
                Console.WriteLine($"\nBileþen stok kontrolleri:");

                var yetersizStoklar = new List<string>();
                foreach (var bilesen in gerekliBilesenler)
                {
                    var bilesenUrun = await _context.Urunler.FindAsync(bilesen.BilesenUrunId);
                    decimal gerekliMiktar = bilesen.Miktar * dto.HedefMiktar;

                    Console.WriteLine($"\n  Bileþen: {bilesenUrun?.Ad}");
                    Console.WriteLine($"  Birim Miktar: {bilesen.Miktar}");
                    Console.WriteLine($"  Toplam Gerekli: {gerekliMiktar}");
                    var mevcutStok = await _context.Stoklar
                        .Where(s => s.DepoId == girdiDepo.Id &&
                                   s.UrunId == bilesen.BilesenUrunId &&
                                   s.Durum == StokDurum.Hazir) // Sadece Hazýr stoklar
                        .SumAsync(s => (decimal?)s.Miktar) ?? 0;

                    Console.WriteLine($"  Mevcut Stok: {mevcutStok}");

                    if (mevcutStok < gerekliMiktar)
                    {
                        var eksik = gerekliMiktar - mevcutStok;
                        var mesaj = $"{bilesenUrun?.Ad}: {gerekliMiktar} adet gerekli, {mevcutStok} adet mevcut (Eksik: {eksik})";
                        yetersizStoklar.Add(mesaj);
                        Console.WriteLine($"  ? YETERSÝZ: {mesaj}");
                    }
                    else
                    {
                        Console.WriteLine($"  ? Yeterli stok var");
                    }
                }
                if (yetersizStoklar.Any())
                {
                    Console.WriteLine("\n? YETERSÝZ STOK - ÝÞ EMRÝ OLUÞTURULAMADI!");
                    await transaction.RollbackAsync();

                    var hataDetay = string.Join("\n", yetersizStoklar);
                    return ResultDto<IsEmriDto>.FailureResult(
                        $"Yetersiz stok! Ýþ emri oluþturulamadý.\n\n{hataDetay}"
                    );
                }

                Console.WriteLine("\n? TÜM BÝLEÞENLER ÝÇÝN YETERLÝ STOK MEVCUT!");
            }
            else
            {
                Console.WriteLine("?? Bileþen yok, stok kontrolü atlanýyor");
            }
            var isEmriNo = await GenerateIsEmriNoAsync();
            Console.WriteLine($"\nÝþ Emri No: {isEmriNo}");
            var isEmri = new IsEmri
            {
                IsEmriNo = isEmriNo,
                UrunId = dto.UrunId,
                HedefMiktar = dto.HedefMiktar,
                TamamlananMiktar = 0,
                HurdaMiktar = 0,
                Durum = IsEmriDurum.Beklemede,
                Notlar = dto.Notlar,
                OlusturanKullaniciId = olusturanKullaniciId,
                CreatedAt = DateTime.Now
            };

            await _context.IsEmirleri.AddAsync(isEmri);
            await _context.SaveChangesAsync();

            Console.WriteLine($"? Ýþ emri oluþturuldu: ID={isEmri.Id}");
            Console.WriteLine("\n=== ÝÞ ADIMLARI OLUÞTURULUYOR ===");

            int sira = 1;
            foreach (var urunAgaciAdim in urunAgaciAdimlari)
            {
                Console.WriteLine($"\n--- Adým {sira}: {urunAgaciAdim.IstasyonTipi} ---");
                var istasyon = await _context.Istasyonlar
                    .FirstOrDefaultAsync(i => i.Tip == urunAgaciAdim.IstasyonTipi);

                if (istasyon == null)
                {
                    Console.WriteLine($"? {urunAgaciAdim.IstasyonTipi} tipinde istasyon bulunamadý!");
                    await transaction.RollbackAsync();
                    return ResultDto<IsEmriDto>.FailureResult(
                        $"{urunAgaciAdim.IstasyonTipi} tipinde istasyon tanýmlý deðil!");
                }

                Console.WriteLine($"Ýstasyon bulundu: {istasyon.Ad} (ID: {istasyon.Id})");
                int? girdiDepoId = null;
                int? girdiUrunId = null;

                if (sira == 1)
                {
                    if (gerekliBilesenler.Any())
                    {
                        var ilkBilesen = gerekliBilesenler.First();
                        var bilesenUrun = await _context.Urunler.FindAsync(ilkBilesen.BilesenUrunId);

                        DepoTip girdiDepoTipi = bilesenUrun?.Tip == UrunTip.YariMamul
                            ? DepoTip.YariMamul
                            : DepoTip.Hammadde;

                        var girdiDepo = await _context.Depolar
                            .FirstOrDefaultAsync(d => d.Tip == girdiDepoTipi);

                        girdiDepoId = girdiDepo?.Id;

                        Console.WriteLine($"? Ýlk adým - Bileþen tipi: {bilesenUrun?.Tip}");
                        Console.WriteLine($"? Girdi Depo: {girdiDepo?.Ad} (Tip: {girdiDepo?.Tip})");
                    }
                    else
                    {
                        girdiDepoId = hammaddeDepo.Id;
                        Console.WriteLine($"Bileþen yok - Girdi Depo: {hammaddeDepo.Ad}");
                    }
                }
                else
                {
                    var oncekiAdim = await _context.IsAdimlari
                        .Where(a => a.IsEmriId == isEmri.Id && a.Sira == sira - 1)
                        .FirstOrDefaultAsync();

                    girdiDepoId = oncekiAdim?.CiktiDepoId;
                    girdiUrunId = oncekiAdim?.CiktiUrunId;

                    var girdiDepo = await _context.Depolar.FindAsync(girdiDepoId);
                    Console.WriteLine($"Sonraki adým - Girdi Depo: {girdiDepo?.Ad ?? "NULL"} (ID: {girdiDepoId})");
                }
                int ciktiUrunId = urunAgaciAdim.CiktiUrunId ?? dto.UrunId;
                var isAdimi = new IsAdimi
                {
                    IsEmriId = isEmri.Id,
                    IstasyonId = istasyon.Id,
                    Sira = sira,
                    Tanim = urunAgaciAdim.Tanim,
                    HedefMiktar = dto.HedefMiktar,
                    TamamlananMiktar = 0,
                    Durum = sira == 1 ? IsAdimiDurum.Hazir : IsAdimiDurum.Beklemede,
                    KaliteKontrolGerekli = urunAgaciAdim.KaliteKontrolGerekli,

                    GirdiDepoId = girdiDepoId,
                    GirdiUrunId = girdiUrunId,

                    CiktiDepoId = urunAgaciAdim.HedefDepoId,
                    CiktiUrunId = ciktiUrunId
                };

                Console.WriteLine($"IstasyonId: {isAdimi.IstasyonId}");
                Console.WriteLine($"GirdiDepoId: {isAdimi.GirdiDepoId}");
                Console.WriteLine($"CiktiDepoId: {isAdimi.CiktiDepoId} › {urunAgaciAdim.HedefDepo.Ad}");
                Console.WriteLine($"Durum: {isAdimi.Durum}");

                await _context.IsAdimlari.AddAsync(isAdimi);
                await _context.SaveChangesAsync();

                Console.WriteLine($"? Ýþ adýmý oluþturuldu: ID={isAdimi.Id}");
                if (sira == 1 && gerekliBilesenler.Any())
                {
                    Console.WriteLine("Gerekli bileþenler ekleniyor...");
                    foreach (var bilesen in gerekliBilesenler)
                    {
                        var isAdimiBilesen = new IsAdimiBilesen
                        {
                            IsAdimiId = isAdimi.Id,
                            BilesenUrunId = bilesen.BilesenUrunId,
                            Miktar = bilesen.Miktar
                        };
                        await _context.IsAdimiBilesenleri.AddAsync(isAdimiBilesen);

                        var bilesenUrun = await _context.Urunler.FindAsync(bilesen.BilesenUrunId);
                        Console.WriteLine($"  ? Bileþen eklendi: {bilesenUrun?.Ad} ({bilesen.Miktar} adet)");
                    }
                    await _context.SaveChangesAsync();
                }

                sira++;
            }

            await transaction.CommitAsync();

            Console.WriteLine("\n??? ÝÞ EMRÝ BAÞARIYLA OLUÞTURULDU!");
            Console.WriteLine("====================================\n");
            var result = new IsEmriDto
            {
                Id = isEmri.Id,
                IsEmriNo = isEmri.IsEmriNo,
                UrunId = isEmri.UrunId,
                UrunAdi = urun.Ad,
                HedefMiktar = isEmri.HedefMiktar,
                TamamlananMiktar = isEmri.TamamlananMiktar,
                HurdaMiktar = isEmri.HurdaMiktar,
                Durum = isEmri.Durum.ToString(),
                OlusturanKullanici = "Admin",
                Notlar = isEmri.Notlar,
                CreatedAt = isEmri.CreatedAt
            };

            return ResultDto<IsEmriDto>.SuccessResult(result, "Ýþ emri baþarýyla oluþturuldu!");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"\n? HATA: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }

            return ResultDto<IsEmriDto>.FailureResult($"Ýþ emri oluþturulamadý: {ex.Message}");
        }
    }
    private async Task<string> GenerateIsEmriNoAsync()
    {
        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;

        var lastIsEmri = await _context.IsEmirleri
            .Where(ie => ie.IsEmriNo.StartsWith($"IE-{year:0000}-{month:00}"))
            .OrderByDescending(ie => ie.IsEmriNo)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastIsEmri != null)
        {
            var parts = lastIsEmri.IsEmriNo.Split('-');
            if (parts.Length == 4 && int.TryParse(parts[3], out int lastSequence))
            {
                sequence = lastSequence + 1;
            }
        }

        return $"IE-{year:0000}-{month:00}-{sequence:0000}";
    }

    public async Task<ResultDto<IsEmriDto>> GetByIdAsync(int id)
    {
        var isEmri = await _context.IsEmirleri
            .Include(i => i.Urun)
            .Include(i => i.OlusturanKullanici)
            .Include(i => i.IsAdimlari)
                .ThenInclude(a => a.Istasyon)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (isEmri == null)
            return ResultDto<IsEmriDto>.FailureResult("Ýþ emri bulunamadý!");

        var dto = new IsEmriDto
        {
            Id = isEmri.Id,
            IsEmriNo = isEmri.IsEmriNo,
            UrunId = isEmri.UrunId,
            UrunAdi = isEmri.Urun.Ad,
            HedefMiktar = isEmri.HedefMiktar,
            TamamlananMiktar = isEmri.TamamlananMiktar,
            HurdaMiktar = isEmri.HurdaMiktar,
            Durum = isEmri.Durum.ToString(),
            BaslangicTarihi = isEmri.BaslangicTarihi,
            BitisTarihi = isEmri.BitisTarihi,
            OlusturanKullanici = isEmri.OlusturanKullanici.AdSoyad ?? isEmri.OlusturanKullanici.KullaniciAdi,
            Notlar = isEmri.Notlar,
            CreatedAt = isEmri.CreatedAt,
            IsAdimlari = isEmri.IsAdimlari.Select(a => new IsAdimiDto
            {
                Id = a.Id,
                Sira = a.Sira,
                IstasyonAdi = a.Istasyon.Ad,
                Tanim = a.Tanim,
                Durum = a.Durum.ToString(),
                HedefMiktar = a.HedefMiktar,
                TamamlananMiktar = a.TamamlananMiktar,
                BaslangicTarihi = a.BaslangicTarihi,
                BitisTarihi = a.BitisTarihi
            }).ToList()
        };

        return ResultDto<IsEmriDto>.SuccessResult(dto);
    }

    public async Task<ResultDto<List<IsEmriDto>>> GetAllAsync()
    {
        var isEmirleri = await _context.IsEmirleri
            .Include(i => i.Urun)
            .Include(i => i.OlusturanKullanici)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        var dtoList = isEmirleri.Select(i => new IsEmriDto
        {
            Id = i.Id,
            IsEmriNo = i.IsEmriNo,
            UrunId = i.UrunId,
            UrunAdi = i.Urun.Ad,
            HedefMiktar = i.HedefMiktar,
            TamamlananMiktar = i.TamamlananMiktar,
            HurdaMiktar = i.HurdaMiktar,
            Durum = i.Durum.ToString(),
            BaslangicTarihi = i.BaslangicTarihi,
            BitisTarihi = i.BitisTarihi,
            OlusturanKullanici = i.OlusturanKullanici.AdSoyad ?? i.OlusturanKullanici.KullaniciAdi,
            CreatedAt = i.CreatedAt
        }).ToList();

        return ResultDto<List<IsEmriDto>>.SuccessResult(dtoList);
    }

    public async Task<ResultDto<List<IsEmriDto>>> GetByDurumAsync(string durum)
    {
        if (!Enum.TryParse<IsEmriDurum>(durum, out var durumEnum))
            return ResultDto<List<IsEmriDto>>.FailureResult("Geçersiz durum!");

        var isEmirleri = await _context.IsEmirleri
            .Include(i => i.Urun)
            .Include(i => i.OlusturanKullanici)
            .Where(i => i.Durum == durumEnum)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        var dtoList = isEmirleri.Select(i => new IsEmriDto
        {
            Id = i.Id,
            IsEmriNo = i.IsEmriNo,
            UrunId = i.UrunId,
            UrunAdi = i.Urun.Ad,
            HedefMiktar = i.HedefMiktar,
            TamamlananMiktar = i.TamamlananMiktar,
            Durum = i.Durum.ToString(),
            BaslangicTarihi = i.BaslangicTarihi,
            CreatedAt = i.CreatedAt
        }).ToList();

        return ResultDto<List<IsEmriDto>>.SuccessResult(dtoList);
    }

    public async Task<ResultDto<bool>> CancelIsEmriAsync(int id, int kullaniciId)
    {
        var isEmri = await _context.IsEmirleri.FindAsync(id);
        if (isEmri == null)
            return ResultDto<bool>.FailureResult("Ýþ emri bulunamadý!");

        if (isEmri.Durum == IsEmriDurum.Tamamlandi)
            return ResultDto<bool>.FailureResult("Tamamlanmýþ iþ emri iptal edilemez!");

        isEmri.Durum = IsEmriDurum.IptalEdildi;
        isEmri.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ResultDto<bool>.SuccessResult(true, "Ýþ emri iptal edildi!");
    }
}