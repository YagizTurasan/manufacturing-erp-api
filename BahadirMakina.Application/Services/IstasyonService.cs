using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.DTOs.IsAdimi;
using BahadirMakina.Application.DTOs.IsEmri;
using BahadirMakina.Application.DTOs.Istasyon;
using BahadirMakina.Application.Interfaces;
using BahadirMakina.Domain.Enums;
using BahadirMakina.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BahadirMakina.Application.Services;

public class IstasyonService : IIstasyonService
{
    private readonly ApplicationDbContext _context;

    public IstasyonService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResultDto<IstasyonDto>> GetByIdAsync(int id)
    {
        try
        {
            var istasyon = await _context.Istasyonlar
                .Include(i => i.Depo)
                .Include(i => i.AktifIsAdimi)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (istasyon == null)
                return ResultDto<IstasyonDto>.FailureResult("Ýstasyon bulunamadý!");

            var bekleyenIsSayisi = await _context.IsAdimlari
                .CountAsync(a =>
                    a.IstasyonId == id &&
                    (a.Durum == IsAdimiDurum.Hazir || a.Durum == IsAdimiDurum.Beklemede));

            var dto = new IstasyonDto
            {
                Id = istasyon.Id,
                Ad = istasyon.Ad,
                Kod = istasyon.Kod,
                Tip = istasyon.Tip.ToString(),
                Durum = istasyon.Durum.ToString(),
                DepoAdi = istasyon.Depo.Ad,
                AktifIsVar = istasyon.AktifIsAdimi != null,
                BekleyenIsSayisi = bekleyenIsSayisi
            };

            return ResultDto<IstasyonDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            return ResultDto<IstasyonDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<IstasyonDto>>> GetAllAsync()
    {
        try
        {
            var istasyonlar = await _context.Istasyonlar
                .Include(i => i.Depo)
                .Include(i => i.AktifIsAdimi)
                .OrderBy(i => i.Tip)
                .ThenBy(i => i.Ad)
                .ToListAsync();

            var dtoList = new List<IstasyonDto>();

            foreach (var istasyon in istasyonlar)
            {
                var bekleyenIsSayisi = await _context.IsAdimlari
                    .CountAsync(a =>
                        a.IstasyonId == istasyon.Id &&
                        (a.Durum == IsAdimiDurum.Hazir || a.Durum == IsAdimiDurum.Beklemede));

                dtoList.Add(new IstasyonDto
                {
                    Id = istasyon.Id,
                    Ad = istasyon.Ad,
                    Kod = istasyon.Kod,
                    Tip = istasyon.Tip.ToString(),
                    Durum = istasyon.Durum.ToString(),
                    DepoAdi = istasyon.Depo.Ad,
                    AktifIsVar = istasyon.AktifIsAdimi != null,
                    BekleyenIsSayisi = bekleyenIsSayisi
                });
            }

            return ResultDto<List<IstasyonDto>>.SuccessResult(dtoList);
        }
        catch (Exception ex)
        {
            return ResultDto<List<IstasyonDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<IstasyonDto>>> GetByTipAsync(IstasyonTip tip)
    {
        try
        {
            var istasyonlar = await _context.Istasyonlar
                .Include(i => i.Depo)
                .Include(i => i.AktifIsAdimi)
                .Where(i => i.Tip == tip)
                .OrderBy(i => i.Ad)
                .ToListAsync();

            var dtoList = new List<IstasyonDto>();

            foreach (var istasyon in istasyonlar)
            {
                var bekleyenIsSayisi = await _context.IsAdimlari
                    .CountAsync(a =>
                        a.IstasyonId == istasyon.Id &&
                        (a.Durum == IsAdimiDurum.Hazir || a.Durum == IsAdimiDurum.Beklemede));

                dtoList.Add(new IstasyonDto
                {
                    Id = istasyon.Id,
                    Ad = istasyon.Ad,
                    Kod = istasyon.Kod,
                    Tip = istasyon.Tip.ToString(),
                    Durum = istasyon.Durum.ToString(),
                    DepoAdi = istasyon.Depo.Ad,
                    AktifIsVar = istasyon.AktifIsAdimi != null,
                    BekleyenIsSayisi = bekleyenIsSayisi
                });
            }

            return ResultDto<List<IstasyonDto>>.SuccessResult(dtoList);
        }
        catch (Exception ex)
        {
            return ResultDto<List<IstasyonDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<IstasyonDto>>> GetByDurumAsync(IstasyonDurum durum)
    {
        try
        {
            var istasyonlar = await _context.Istasyonlar
                .Include(i => i.Depo)
                .Include(i => i.AktifIsAdimi)
                .Where(i => i.Durum == durum)
                .OrderBy(i => i.Tip)
                .ThenBy(i => i.Ad)
                .ToListAsync();

            var dtoList = new List<IstasyonDto>();

            foreach (var istasyon in istasyonlar)
            {
                var bekleyenIsSayisi = await _context.IsAdimlari
                    .CountAsync(a =>
                        a.IstasyonId == istasyon.Id &&
                        (a.Durum == IsAdimiDurum.Hazir || a.Durum == IsAdimiDurum.Beklemede));

                dtoList.Add(new IstasyonDto
                {
                    Id = istasyon.Id,
                    Ad = istasyon.Ad,
                    Kod = istasyon.Kod,
                    Tip = istasyon.Tip.ToString(),
                    Durum = istasyon.Durum.ToString(),
                    DepoAdi = istasyon.Depo.Ad,
                    AktifIsVar = istasyon.AktifIsAdimi != null,
                    BekleyenIsSayisi = bekleyenIsSayisi
                });
            }

            return ResultDto<List<IstasyonDto>>.SuccessResult(dtoList);
        }
        catch (Exception ex)
        {
            return ResultDto<List<IstasyonDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<IstasyonDetayDto>> GetDetayByIdAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var istasyon = await _context.Istasyonlar
                .Include(i => i.Depo)
                .Include(i => i.AktifIsAdimi)
                    .ThenInclude(a => a!.IsEmri)
                        .ThenInclude(e => e.Urun)
                .Include(i => i.AktifIsAdimi)
                    .ThenInclude(a => a!.SorumluKullanici)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (istasyon == null)
                return ResultDto<IstasyonDetayDto>.FailureResult("Ýstasyon bulunamadý!");
            AktifIsDto? aktifIs = null;
            if (istasyon.AktifIsAdimi != null)
            {
                var gecenSure = DateTime.UtcNow - istasyon.AktifIsAdimi.BaslangicTarihi!.Value;
                aktifIs = new AktifIsDto
                {
                    IsAdimiId = istasyon.AktifIsAdimi.Id,
                    IsEmriNo = istasyon.AktifIsAdimi.IsEmri.IsEmriNo,
                    UrunAdi = istasyon.AktifIsAdimi.IsEmri.Urun.Ad,
                    Tanim = istasyon.AktifIsAdimi.Tanim,
                    HedefMiktar = istasyon.AktifIsAdimi.HedefMiktar,
                    TamamlananMiktar = istasyon.AktifIsAdimi.TamamlananMiktar,
                    BaslangicTarihi = istasyon.AktifIsAdimi.BaslangicTarihi.Value,
                    GecenSure = gecenSure,
                    SorumluKullanici = istasyon.AktifIsAdimi.SorumluKullanici?.AdSoyad ??
                                       istasyon.AktifIsAdimi.SorumluKullanici?.KullaniciAdi ?? "Bilinmiyor"
                };
            }
            var bekleyenIsler = await _context.IsAdimlari
                .Include(a => a.IsEmri)
                    .ThenInclude(e => e.Urun)
                .Include(a => a.GerekliBilesenler)
                    .ThenInclude(b => b.Urun)
                .Include(a => a.GirdiDepo)
                .Where(a =>
                    a.IstasyonId == id &&
                    (a.Durum == IsAdimiDurum.Hazir || a.Durum == IsAdimiDurum.Beklemede))
                .OrderBy(a => a.IsEmri.CreatedAt)
                .ToListAsync();

            var bekleyenIslerDto = new List<BekleyenIsDto>();
            foreach (var isAdimi in bekleyenIsler)
            {
                var dto = new BekleyenIsDto
                {
                    IsAdimiId = isAdimi.Id,
                    IsEmriNo = isAdimi.IsEmri.IsEmriNo,
                    UrunAdi = isAdimi.IsEmri.Urun.Ad,
                    HedefMiktar = isAdimi.HedefMiktar,
                    Tanim = isAdimi.Tanim
                };
                if (isAdimi.GerekliBilesenler.Any())
                {
                    dto.GerekliBilesenler = new List<GerekliBilesenDto>();
                    foreach (var bilesen in isAdimi.GerekliBilesenler)
                    {
                        var stok = await _context.Stoklar
                            .FirstOrDefaultAsync(s =>
                                s.DepoId == isAdimi.GirdiDepoId &&
                                s.UrunId == bilesen.BilesenUrunId);

                        dto.GerekliBilesenler.Add(new GerekliBilesenDto
                        {
                            UrunAdi = bilesen.Urun.Ad,
                            Miktar = bilesen.Miktar,
                            StokYeterli = stok != null && stok.Miktar >= bilesen.Miktar
                        });
                    }
                }

                bekleyenIslerDto.Add(dto);
            }
            var bugun = DateTime.Today;
            var haftaninBaslangici = bugun.AddDays(-(int)bugun.DayOfWeek + (int)DayOfWeek.Monday);
            var ayinBaslangici = new DateTime(bugun.Year, bugun.Month, 1);

            var tamamlananIsler = await _context.IsAdimlari
                .Where(a => a.IstasyonId == id && a.Durum == IsAdimiDurum.Tamamlandi)
                .ToListAsync();

            var bugunkuIsler = tamamlananIsler
                .Where(a => a.BitisTarihi.HasValue && a.BitisTarihi.Value.Date == bugun)
                .ToList();

            var haftaninIsleri = tamamlananIsler
                .Where(a => a.BitisTarihi.HasValue && a.BitisTarihi.Value >= haftaninBaslangici)
                .ToList();

            var ayinIsleri = tamamlananIsler
                .Where(a => a.BitisTarihi.HasValue && a.BitisTarihi.Value >= ayinBaslangici)
                .ToList();
            var toplamCalismaZamani = tamamlananIsler
                .Where(a => a.BaslangicTarihi.HasValue && a.BitisTarihi.HasValue)
                .Sum(a => (decimal)(a.BitisTarihi!.Value - a.BaslangicTarihi!.Value).TotalHours);
            var ortalamaCevrimeZamani = tamamlananIsler
                .Where(a => a.BaslangicTarihi.HasValue && a.BitisTarihi.HasValue)
                .Select(a => (decimal)(a.BitisTarihi!.Value - a.BaslangicTarihi!.Value).TotalMinutes)
                .DefaultIfEmpty(0)
                .Average();

            var istatistikler = new IstatistikDto
            {
                BugunkuIsAdedi = bugunkuIsler.Count,
                TamamlananIsAdedi = tamamlananIsler.Count,
                BekleyenIsAdedi = bekleyenIsler.Count,
                ToplamCalismaZamani = Math.Round(toplamCalismaZamani, 2),
                OrtalamaCevrimeZamani = Math.Round(ortalamaCevrimeZamani, 2),
                BuHaftaIsSayisi = haftaninIsleri.Count,
                BuAyIsSayisi = ayinIsleri.Count
            };

            var detayDto = new IstasyonDetayDto
            {
                Id = istasyon.Id,
                Ad = istasyon.Ad,
                Kod = istasyon.Kod,
                Tip = istasyon.Tip.ToString(),
                Durum = istasyon.Durum.ToString(),
                DepoAdi = istasyon.Depo.Ad,
                AktifIs = aktifIs,
                BekleyenIsler = bekleyenIslerDto,
                Istatistikler = istatistikler
            };

            return ResultDto<IstasyonDetayDto>.SuccessResult(detayDto);
        }
        catch (Exception ex)
        {
            if (transaction != null)
                await transaction.RollbackAsync();
            var errorMessage = $"Hata: {ex.Message}";

            if (ex.InnerException != null)
            {
                errorMessage += $"\nInner Exception: {ex.InnerException.Message}";

                if (ex.InnerException.InnerException != null)
                {
                    errorMessage += $"\nInner Inner Exception: {ex.InnerException.InnerException.Message}";
                }
            }
            Console.WriteLine("==========================================");
            Console.WriteLine("? ÝÞ EMRÝ OLUÞTURMA HATASI");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                Console.WriteLine($"InnerException StackTrace: {ex.InnerException.StackTrace}");
            }
            Console.WriteLine("==========================================");

            return ResultDto<IstasyonDetayDto>.FailureResult(errorMessage);
        }
    }

    public async Task<ResultDto<bool>> UpdateDurumAsync(UpdateIstasyonDurumDto dto)
    {
        try
        {
            var istasyon = await _context.Istasyonlar.FindAsync(dto.IstasyonId);
            if (istasyon == null)
                return ResultDto<bool>.FailureResult("Ýstasyon bulunamadý!");

            if (!Enum.TryParse<IstasyonDurum>(dto.Durum, out var yeniDurum))
                return ResultDto<bool>.FailureResult("Geçersiz durum!");

            var eskiDurum = istasyon.Durum;
            istasyon.Durum = yeniDurum;
            istasyon.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ResultDto<bool>.SuccessResult(
                true,
                $"Ýstasyon durumu {eskiDurum} › {yeniDurum} olarak güncellendi"
            );
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<IstasyonDto>>> GetDashboardOzetAsync()
    {
        try
        {
            var istasyonlar = await _context.Istasyonlar
                .Include(i => i.Depo)
                .Include(i => i.AktifIsAdimi)
                .Where(i => i.Durum != IstasyonDurum.Musait)
                .OrderBy(i => i.Durum)
                .ThenBy(i => i.Tip)
                .ToListAsync();

            var dtoList = new List<IstasyonDto>();

            foreach (var istasyon in istasyonlar)
            {
                var bekleyenIsSayisi = await _context.IsAdimlari
                    .CountAsync(a =>
                        a.IstasyonId == istasyon.Id &&
                        (a.Durum == IsAdimiDurum.Hazir || a.Durum == IsAdimiDurum.Beklemede));

                dtoList.Add(new IstasyonDto
                {
                    Id = istasyon.Id,
                    Ad = istasyon.Ad,
                    Kod = istasyon.Kod,
                    Tip = istasyon.Tip.ToString(),
                    Durum = istasyon.Durum.ToString(),
                    DepoAdi = istasyon.Depo.Ad,
                    AktifIsVar = istasyon.AktifIsAdimi != null,
                    BekleyenIsSayisi = bekleyenIsSayisi
                });
            }

            return ResultDto<List<IstasyonDto>>.SuccessResult(dtoList);
        }
        catch (Exception ex)
        {
            return ResultDto<List<IstasyonDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }
}