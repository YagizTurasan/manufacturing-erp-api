using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.DTOs.Urun;
using BahadirMakina.Application.Interfaces;
using BahadirMakina.Domain.Entities;
using BahadirMakina.Domain.Enums;
using BahadirMakina.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BahadirMakina.Application.Services;

public class UrunService : IUrunService
{
    private readonly ApplicationDbContext _context;

    public UrunService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResultDto<UrunDto>> CreateAsync(CreateUrunDto dto)
    {
        try
        {
            var mevcutUrun = await _context.Urunler
                .FirstOrDefaultAsync(u => u.Kod == dto.Kod);

            if (mevcutUrun != null)
                return ResultDto<UrunDto>.FailureResult("Bu ürün kodu zaten kullanýlýyor!");
            if (!Enum.TryParse<UrunTip>(dto.Tip, out var urunTip))
                return ResultDto<UrunDto>.FailureResult("Geçersiz ürün tipi!");
            var urun = new Urun
            {
                Ad = dto.Ad,
                Kod = dto.Kod,
                Tip = urunTip,
                Birim = dto.Birim,
                MinimumStok = dto.MinimumStok,
                CreatedAt = DateTime.UtcNow
            };

            _context.Urunler.Add(urun);
            await _context.SaveChangesAsync();

            var responseDto = new UrunDto
            {
                Id = urun.Id,
                Ad = urun.Ad,
                Kod = urun.Kod,
                Tip = urun.Tip.ToString(),
                Birim = urun.Birim,
                MinimumStok = urun.MinimumStok,
                ToplamStok = 0,
                AdimSayisi = 0,
                CreatedAt = urun.CreatedAt
            };

            return ResultDto<UrunDto>.SuccessResult(responseDto, "Ürün oluþturuldu!");
        }
        catch (Exception ex)
        {
            return ResultDto<UrunDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<UrunDto>> GetByIdAsync(int id)
    {
        try
        {
            var urun = await _context.Urunler
                .Include(u => u.Stoklar)
                .Include(u => u.UrunAgaciAdimlari)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (urun == null)
                return ResultDto<UrunDto>.FailureResult("Ürün bulunamadý!");

            var dto = new UrunDto
            {
                Id = urun.Id,
                Ad = urun.Ad,
                Kod = urun.Kod,
                Tip = urun.Tip.ToString(),
                Birim = urun.Birim,
                MinimumStok = urun.MinimumStok,
                ToplamStok = urun.Stoklar.Sum(s => s.Miktar),
                AdimSayisi = urun.UrunAgaciAdimlari.Count,
                CreatedAt = urun.CreatedAt
            };

            return ResultDto<UrunDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            return ResultDto<UrunDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<UrunDetayDto>> GetDetayByIdAsync(int id)
    {
        try
        {
            var urun = await _context.Urunler
            .Include(u => u.Stoklar)
                .ThenInclude(s => s.Depo)
            .Include(u => u.UrunAgaciAdimlari)
                .ThenInclude(a => a.HedefDepo)
            .Include(u => u.UrunAgaciAdimlari)
                .ThenInclude(a => a.CiktiUrun)
            .Include(u => u.GerekliOlduguUrunler) // ? ThenInclude YOK!
            .FirstOrDefaultAsync(u => u.Id == id);

            if (urun == null)
                return ResultDto<UrunDetayDto>.FailureResult("Ürün bulunamadý!");

            Console.WriteLine($"=== ÜRÜN DETAY DEBUG ===");
            Console.WriteLine($"Ürün: {urun.Ad} ({urun.Id})");
            Console.WriteLine($"GereklýOlduguUrunler Count: {urun.GerekliOlduguUrunler.Count}");
            var stoklar = urun.Stoklar
                .Where(s => s.Miktar > 0)
                .Select(s => new StokDetayDto
                {
                    DepoAdi = s.Depo.Ad,
                    Miktar = s.Miktar,
                    Durum = s.Durum.ToString()
                })
                .ToList();
            var adimlar = urun.UrunAgaciAdimlari
                .OrderBy(a => a.Sira)
                .Select(a => new UrunAgaciAdimDto
                {
                    Id = a.Id,
                    Sira = a.Sira,
                    IstasyonTipi = a.IstasyonTipi.ToString(),
                    Tanim = a.Tanim,
                    CiktiUrunAdi = a.CiktiUrun?.Ad,
                    KaliteKontrolGerekli = a.KaliteKontrolGerekli,
                    HedefDepoAdi = a.HedefDepo?.Ad
                })
                .ToList();
            var bilesenUrunIdleri = urun.BilesenOlduguUrunler
                .Select(b => b.BilesenUrunId)
                .ToList();

            var bilesenStoklar = await _context.Stoklar
                .Where(s => bilesenUrunIdleri.Contains(s.UrunId))
                .GroupBy(s => s.UrunId)
                .Select(g => new
                {
                    UrunId = g.Key,
                    ToplamStok = g.Sum(s => s.Miktar)
                })
                .ToDictionaryAsync(x => x.UrunId, x => x.ToplamStok);

            var bilesenler = urun.BilesenOlduguUrunler
                .Select(b => new GerekliBilesenDetayDto
                {
                    Id = b.Id,
                    BilesenAdi = b.BilesenUrun.Ad,
                    BilesenKodu = b.BilesenUrun.Kod,
                    Miktar = b.Miktar,
                    MevcutStok = bilesenStoklar.ContainsKey(b.BilesenUrunId)
                        ? bilesenStoklar[b.BilesenUrunId]
                        : 0
                })
                .ToList();
            var toplamUretilen = await _context.IsEmirleri
                .Where(i => i.UrunId == id && i.Durum == IsEmriDurum.Tamamlandi)
                .SumAsync(i => i.TamamlananMiktar);

            var toplamHurda = await _context.Hurdalar
                .Where(h => h.UrunId == id)
                .SumAsync(h => h.Miktar);

            var aktifIsler = await _context.IsEmirleri
                .CountAsync(i => i.UrunId == id && i.Durum == IsEmriDurum.DevamEdiyor);

            var hurdaOrani = toplamUretilen > 0
                ? (toplamHurda / (toplamUretilen + toplamHurda)) * 100
                : 0;

            var istatistikler = new UretimIstatistikDto
            {
                ToplamUretilenAdet = (int)toplamUretilen,
                ToplamHurdaAdet = (int)toplamHurda,
                HurdaOrani = Math.Round(hurdaOrani, 2),
                AktifIsEmriSayisi = aktifIsler
            };

            var detayDto = new UrunDetayDto
            {
                Id = urun.Id,
                Ad = urun.Ad,
                Kod = urun.Kod,
                Tip = urun.Tip.ToString(),
                Birim = urun.Birim,
                MinimumStok = urun.MinimumStok,
                Stoklar = stoklar,
                UretimAdimlari = adimlar,
                GerekliBilesenler = bilesenler,
                Istatistikler = istatistikler
            };

            return ResultDto<UrunDetayDto>.SuccessResult(detayDto);
        }
        catch (Exception ex)
        {
            return ResultDto<UrunDetayDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<UrunDto>>> GetAllAsync()
    {
        try
        {
            var urunler = await _context.Urunler
                .Include(u => u.Stoklar)
                .Include(u => u.UrunAgaciAdimlari)
                .OrderBy(u => u.Tip)
                .ThenBy(u => u.Ad)
                .ToListAsync();

            var dtoList = urunler.Select(u => new UrunDto
            {
                Id = u.Id,
                Ad = u.Ad,
                Kod = u.Kod,
                Tip = u.Tip.ToString(),
                Birim = u.Birim,
                MinimumStok = u.MinimumStok,
                ToplamStok = u.Stoklar.Sum(s => s.Miktar),
                AdimSayisi = u.UrunAgaciAdimlari.Count,
                CreatedAt = u.CreatedAt
            }).ToList();

            return ResultDto<List<UrunDto>>.SuccessResult(dtoList);
        }
        catch (Exception ex)
        {
            return ResultDto<List<UrunDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<UrunDto>>> GetByTipAsync(string tip)
    {
        try
        {
            if (!Enum.TryParse<UrunTip>(tip, out var urunTip))
                return ResultDto<List<UrunDto>>.FailureResult("Geçersiz ürün tipi!");

            var urunler = await _context.Urunler
                .Include(u => u.Stoklar)
                .Include(u => u.UrunAgaciAdimlari)
                .Where(u => u.Tip == urunTip)
                .OrderBy(u => u.Ad)
                .ToListAsync();

            var dtoList = urunler.Select(u => new UrunDto
            {
                Id = u.Id,
                Ad = u.Ad,
                Kod = u.Kod,
                Tip = u.Tip.ToString(),
                Birim = u.Birim,
                MinimumStok = u.MinimumStok,
                ToplamStok = u.Stoklar.Sum(s => s.Miktar),
                AdimSayisi = u.UrunAgaciAdimlari.Count,
                CreatedAt = u.CreatedAt
            }).ToList();

            return ResultDto<List<UrunDto>>.SuccessResult(dtoList);
        }
        catch (Exception ex)
        {
            return ResultDto<List<UrunDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<UrunDto>> UpdateAsync(UpdateUrunDto dto)
    {
        try
        {
            var urun = await _context.Urunler.FindAsync(dto.Id);

            if (urun == null)
                return ResultDto<UrunDto>.FailureResult("Ürün bulunamadý!");
            if (!string.IsNullOrWhiteSpace(dto.Kod) && dto.Kod != urun.Kod)
            {
                var mevcutUrun = await _context.Urunler
                    .FirstOrDefaultAsync(u => u.Kod == dto.Kod && u.Id != dto.Id);

                if (mevcutUrun != null)
                    return ResultDto<UrunDto>.FailureResult("Bu ürün kodu zaten kullanýlýyor!");

                urun.Kod = dto.Kod;
            }

            if (!string.IsNullOrWhiteSpace(dto.Ad))
                urun.Ad = dto.Ad;

            if (!string.IsNullOrWhiteSpace(dto.Birim))
                urun.Birim = dto.Birim;

            if (dto.MinimumStok.HasValue)
                urun.MinimumStok = dto.MinimumStok;

            urun.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var toplamStok = await _context.Stoklar
                .Where(s => s.UrunId == urun.Id)
                .SumAsync(s => s.Miktar);

            var adimSayisi = await _context.UrunAgaciAdimlari
                .CountAsync(a => a.UrunId == urun.Id);

            var responseDto = new UrunDto
            {
                Id = urun.Id,
                Ad = urun.Ad,
                Kod = urun.Kod,
                Tip = urun.Tip.ToString(),
                Birim = urun.Birim,
                MinimumStok = urun.MinimumStok,
                ToplamStok = toplamStok,
                AdimSayisi = adimSayisi,
                CreatedAt = urun.CreatedAt
            };

            return ResultDto<UrunDto>.SuccessResult(responseDto, "Ürün güncellendi!");
        }
        catch (Exception ex)
        {
            return ResultDto<UrunDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<bool>> DeleteAsync(int id)
    {
        try
        {
            var urun = await _context.Urunler
                .Include(u => u.Stoklar)
                .Include(u => u.UrunAgaciAdimlari)
                .Include(u => u.GerekliOlduguUrunler)  // ? DÜZELTME!
                .FirstOrDefaultAsync(u => u.Id == id);

            if (urun == null)
                return ResultDto<bool>.FailureResult("Ürün bulunamadý!");
            var aktifIsler = await _context.IsEmirleri
                .AnyAsync(i => i.UrunId == id && i.Durum != IsEmriDurum.Tamamlandi && i.Durum != IsEmriDurum.IptalEdildi);

            if (aktifIsler)
                return ResultDto<bool>.FailureResult("Bu ürün için aktif iþ emirleri var, silinemez!");
            _context.UrunAgaciAdimlari.RemoveRange(urun.UrunAgaciAdimlari);
            _context.UrunAgaciGerekliBilesenler.RemoveRange(urun.GerekliOlduguUrunler);  // ? DÜZELTME!
            _context.Stoklar.RemoveRange(urun.Stoklar);
            _context.Urunler.Remove(urun);

            await _context.SaveChangesAsync();

            return ResultDto<bool>.SuccessResult(true, "Ürün silindi!");
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<UrunAgaciAdimDto>> CreateAdimAsync(CreateUrunAgaciAdimDto dto)
    {
        try
        {
            var urun = await _context.Urunler.FindAsync(dto.UrunId);
            if (urun == null)
                return ResultDto<UrunAgaciAdimDto>.FailureResult("Ürün bulunamadý!");

            if (!Enum.TryParse<IstasyonTip>(dto.IstasyonTipi, out var istasyonTip))
                return ResultDto<UrunAgaciAdimDto>.FailureResult("Geçersiz istasyon tipi!");

            var hedefDepo = await _context.Depolar.FindAsync(dto.HedefDepoId);
            if (hedefDepo == null)
                return ResultDto<UrunAgaciAdimDto>.FailureResult("Hedef depo bulunamadý!");

            var adim = new UrunAgaciAdim
            {
                UrunId = dto.UrunId,
                Sira = dto.Sira,
                IstasyonTipi = istasyonTip,
                Tanim = dto.Tanim,
                CiktiUrunId = dto.CiktiUrunId,
                KaliteKontrolGerekli = dto.KaliteKontrolGerekli,
                HedefDepoId = dto.HedefDepoId,
                CreatedAt = DateTime.UtcNow
            };

            _context.UrunAgaciAdimlari.Add(adim);
            await _context.SaveChangesAsync();

            var ciktiUrun = dto.CiktiUrunId.HasValue 
                ? await _context.Urunler.FindAsync(dto.CiktiUrunId) 
                : null;

            var responseDto = new UrunAgaciAdimDto
            {
                Id = adim.Id,
                Sira = adim.Sira,
                IstasyonTipi = adim.IstasyonTipi.ToString(),
                Tanim = adim.Tanim,
                CiktiUrunAdi = ciktiUrun?.Ad,
                KaliteKontrolGerekli = adim.KaliteKontrolGerekli,
                HedefDepoAdi = hedefDepo.Ad
            };

            return ResultDto<UrunAgaciAdimDto>.SuccessResult(responseDto, "Üretim adýmý eklendi!");
        }
        catch (Exception ex)
        {
            return ResultDto<UrunAgaciAdimDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<bool>> DeleteAdimAsync(int adimId)
    {
        try
        {
            var adim = await _context.UrunAgaciAdimlari.FindAsync(adimId);

            if (adim == null)
                return ResultDto<bool>.FailureResult("Adým bulunamadý!");

            _context.UrunAgaciAdimlari.Remove(adim);
            await _context.SaveChangesAsync();

            return ResultDto<bool>.SuccessResult(true, "Üretim adýmý silindi!");
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<UrunAgaciAdimDto>>> GetAdimlarByUrunIdAsync(int urunId)
    {
        try
        {
            var adimlar = await _context.UrunAgaciAdimlari
                .Include(a => a.CiktiUrun)
                .Include(a => a.HedefDepo)
                .Where(a => a.UrunId == urunId)
                .OrderBy(a => a.Sira)
                .Select(a => new UrunAgaciAdimDto
                {
                    Id = a.Id,
                    Sira = a.Sira,
                    IstasyonTipi = a.IstasyonTipi.ToString(),
                    Tanim = a.Tanim,
                    CiktiUrunAdi = a.CiktiUrun != null ? a.CiktiUrun.Ad : null,
                    KaliteKontrolGerekli = a.KaliteKontrolGerekli,
                    HedefDepoAdi = a.HedefDepo.Ad
                })
                .ToListAsync();

            return ResultDto<List<UrunAgaciAdimDto>>.SuccessResult(adimlar);
        }
        catch (Exception ex)
        {
            return ResultDto<List<UrunAgaciAdimDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<GerekliBilesenDetayDto>> CreateGerekliBilesenAsync(CreateGerekliBilesenDto dto)
    {
        try
        {
            var urun = await _context.Urunler.FindAsync(dto.UrunId);
            if (urun == null)
                return ResultDto<GerekliBilesenDetayDto>.FailureResult("Ürün bulunamadý!");

            var bilesenUrun = await _context.Urunler
                .Include(u => u.Stoklar)
                .FirstOrDefaultAsync(u => u.Id == dto.BilesenUrunId);

            if (bilesenUrun == null)
                return ResultDto<GerekliBilesenDetayDto>.FailureResult("Bileþen ürün bulunamadý!");

            var bilesen = new UrunAgaciGerekliBilesen
            {
                UrunId = dto.UrunId,
                BilesenUrunId = dto.BilesenUrunId,
                Miktar = dto.Miktar,
                CreatedAt = DateTime.UtcNow
            };

            _context.UrunAgaciGerekliBilesenler.Add(bilesen);
            await _context.SaveChangesAsync();

            var responseDto = new GerekliBilesenDetayDto
            {
                Id = bilesen.Id,
                BilesenAdi = bilesenUrun.Ad,
                BilesenKodu = bilesenUrun.Kod,
                Miktar = bilesen.Miktar,
                MevcutStok = bilesenUrun.Stoklar.Sum(s => s.Miktar)
            };

            return ResultDto<GerekliBilesenDetayDto>.SuccessResult(responseDto, "Bileþen eklendi!");
        }
        catch (Exception ex)
        {
            return ResultDto<GerekliBilesenDetayDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<bool>> DeleteGerekliBilesenAsync(int bilesenId)
    {
        try
        {
            var bilesen = await _context.UrunAgaciGerekliBilesenler.FindAsync(bilesenId);

            if (bilesen == null)
                return ResultDto<bool>.FailureResult("Bileþen bulunamadý!");

            _context.UrunAgaciGerekliBilesenler.Remove(bilesen);
            await _context.SaveChangesAsync();

            return ResultDto<bool>.SuccessResult(true, "Bileþen silindi!");
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<GerekliBilesenDetayDto>>> GetGerekliBilesenlerByUrunIdAsync(int urunId)
    {
        try
        {
            var bilesenler = await _context.UrunAgaciGerekliBilesenler
                .Include(b => b.BilesenUrun)
                    .ThenInclude(u => u.Stoklar)
                .Where(b => b.UrunId == urunId)
                .Select(b => new GerekliBilesenDetayDto
                {
                    Id = b.Id,
                    BilesenAdi = b.BilesenUrun.Ad,
                    BilesenKodu = b.BilesenUrun.Kod,
                    Miktar = b.Miktar,
                    MevcutStok = b.BilesenUrun.Stoklar.Sum(s => s.Miktar)
                })
                .ToListAsync();

            return ResultDto<List<GerekliBilesenDetayDto>>.SuccessResult(bilesenler);
        }
        catch (Exception ex)
        {
            return ResultDto<List<GerekliBilesenDetayDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<UrunDto>>> GetDusukStokUrunlerAsync()
    {
        try
        {
            var urunler = await _context.Urunler
                .Include(u => u.Stoklar)
                .Include(u => u.UrunAgaciAdimlari)
                .Where(u => u.MinimumStok.HasValue)
                .ToListAsync();

            var dusukStoklar = urunler
                .Where(u => u.Stoklar.Sum(s => s.Miktar) < u.MinimumStok)
                .Select(u => new UrunDto
                {
                    Id = u.Id,
                    Ad = u.Ad,
                    Kod = u.Kod,
                    Tip = u.Tip.ToString(),
                    Birim = u.Birim,
                    MinimumStok = u.MinimumStok,
                    ToplamStok = u.Stoklar.Sum(s => s.Miktar),
                    AdimSayisi = u.UrunAgaciAdimlari.Count,
                    CreatedAt = u.CreatedAt
                })
                .ToList();

            return ResultDto<List<UrunDto>>.SuccessResult(dusukStoklar);
        }
        catch (Exception ex)
        {
            return ResultDto<List<UrunDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }
}