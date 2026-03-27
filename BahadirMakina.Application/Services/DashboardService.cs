using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.DTOs.Dashboard;
using BahadirMakina.Application.Interfaces;
using BahadirMakina.Domain.Enums;
using BahadirMakina.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BahadirMakina.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResultDto<DashboardOzetDto>> GetOzetAsync()
    {
        try
        {
            var bugun = DateTime.Today;
            var haftaninBaslangici = bugun.AddDays(-(int)bugun.DayOfWeek + (int)DayOfWeek.Monday);
            var ayinBaslangici = new DateTime(bugun.Year, bugun.Month, 1);
            var isEmirleriOzet = new IsEmriOzetDto
            {
                ToplamAktif = await _context.IsEmirleri
                    .CountAsync(i => i.Durum != IsEmriDurum.Tamamlandi && i.Durum != IsEmriDurum.IptalEdildi),

                Beklemede = await _context.IsEmirleri
                    .CountAsync(i => i.Durum == IsEmriDurum.Beklemede),

                DevamEdiyor = await _context.IsEmirleri
                    .CountAsync(i => i.Durum == IsEmriDurum.DevamEdiyor),

                BugunkuTamamlanan = await _context.IsEmirleri
                    .CountAsync(i => i.Durum == IsEmriDurum.Tamamlandi &&
                                    i.BitisTarihi.HasValue &&
                                    i.BitisTarihi.Value.Date == bugun),

                BuHaftaTamamlanan = await _context.IsEmirleri
                    .CountAsync(i => i.Durum == IsEmriDurum.Tamamlandi &&
                                    i.BitisTarihi.HasValue &&
                                    i.BitisTarihi.Value >= haftaninBaslangici),

                BuAyTamamlanan = await _context.IsEmirleri
                    .CountAsync(i => i.Durum == IsEmriDurum.Tamamlandi &&
                                    i.BitisTarihi.HasValue &&
                                    i.BitisTarihi.Value >= ayinBaslangici)
            };
            var istasyonlar = await _context.Istasyonlar.ToListAsync();
            var toplamIstasyon = istasyonlar.Count;
            var musaitSayisi = istasyonlar.Count(i => i.Durum == IstasyonDurum.Musait);
            var mesgulSayisi = istasyonlar.Count(i => i.Durum == IstasyonDurum.Mesgul);
            var arizaliSayisi = istasyonlar.Count(i => i.Durum == IstasyonDurum.Arizali);

            var istasyonlarOzet = new IstasyonOzetDto
            {
                ToplamIstasyon = toplamIstasyon,
                Musait = musaitSayisi,
                Mesgul = mesgulSayisi,
                Arizali = arizaliSayisi,
                KullanimOrani = toplamIstasyon > 0
                    ? Math.Round((decimal)mesgulSayisi / toplamIstasyon * 100, 2)
                    : 0
            };
            var bugunkuIsEmirleri = await _context.IsEmirleri
                .Where(i => i.CreatedAt.Date == bugun)
                .ToListAsync();

            var bugunkuTamamlananMiktar = await _context.IsEmirleri
                .Where(i => i.BitisTarihi.HasValue && i.BitisTarihi.Value.Date == bugun)
                .SumAsync(i => i.TamamlananMiktar);

            var bugunkuHedefMiktar = bugunkuIsEmirleri.Sum(i => i.HedefMiktar);

            var buHaftaUretim = await _context.IsEmirleri
                .Where(i => i.BitisTarihi.HasValue && i.BitisTarihi.Value >= haftaninBaslangici)
                .SumAsync(i => (int)i.TamamlananMiktar);

            var buAyUretim = await _context.IsEmirleri
                .Where(i => i.BitisTarihi.HasValue && i.BitisTarihi.Value >= ayinBaslangici)
                .SumAsync(i => (int)i.TamamlananMiktar);

            var bekleyenKaliteKontrol = await _context.IsAdimlari
                .CountAsync(a => a.Durum == IsAdimiDurum.KaliteKontrolde);

            var uretimOzet = new UretimOzetDto
            {
                BugunkuUretim = (int)bugunkuTamamlananMiktar,
                BuHaftaUretim = buHaftaUretim,
                BuAyUretim = buAyUretim,
                BugunkuHedefTutturmaOrani = bugunkuHedefMiktar > 0
                    ? Math.Round(bugunkuTamamlananMiktar / bugunkuHedefMiktar * 100, 2)
                    : 0,
                BekleyenKaliteKontrol = bekleyenKaliteKontrol
            };
            var bugunkuHurdalar = await _context.Hurdalar
                .Where(h => h.HurdaTarihi.Date == bugun)
                .ToListAsync();

            var buHaftaHurdalar = await _context.Hurdalar
                .Where(h => h.HurdaTarihi >= haftaninBaslangici)
                .ToListAsync();

            var buAyHurdalar = await _context.Hurdalar
                .Where(h => h.HurdaTarihi >= ayinBaslangici)
                .ToListAsync();

            var bugunkuHurdaAdet = bugunkuHurdalar.Sum(h => (int)h.Miktar);
            var buHaftaHurdaAdet = buHaftaHurdalar.Sum(h => (int)h.Miktar);
            var buAyHurdaAdet = buAyHurdalar.Sum(h => (int)h.Miktar);

            var bugunkuHurdaOrani = bugunkuTamamlananMiktar + bugunkuHurdaAdet > 0
                ? Math.Round((decimal)bugunkuHurdaAdet / (bugunkuTamamlananMiktar + bugunkuHurdaAdet) * 100, 2)
                : 0;

            var buAyHurdaOrani = buAyUretim + buAyHurdaAdet > 0
                ? Math.Round((decimal)buAyHurdaAdet / (buAyUretim + buAyHurdaAdet) * 100, 2)
                : 0;

            var hurdaOzet = new HurdaOzetDto
            {
                BugunkuHurdaAdet = bugunkuHurdaAdet,
                BuHaftaHurdaAdet = buHaftaHurdaAdet,
                BuAyHurdaAdet = buAyHurdaAdet,
                BugunkuHurdaOrani = bugunkuHurdaOrani,
                BuAyHurdaOrani = buAyHurdaOrani,
                TahminiMaliyetKaybi = buAyHurdalar.Sum(h => h.TahminiMaliyetKaybi ?? 0)
            };
            var aktifIsler = await _context.IsAdimlari
                .Include(a => a.IsEmri)
                    .ThenInclude(e => e.Urun)
                .Include(a => a.Istasyon)
                .Include(a => a.SorumluKullanici)
                .Where(a => a.Durum == IsAdimiDurum.DevamEdiyor)
                .Select(a => new AktifIsDto
                {
                    IsEmriNo = a.IsEmri.IsEmriNo,
                    UrunAdi = a.IsEmri.Urun.Ad,
                    IstasyonAdi = a.Istasyon.Ad,
                    OperatorAdi = a.SorumluKullanici != null
                        ? (a.SorumluKullanici.AdSoyad ?? a.SorumluKullanici.KullaniciAdi)
                        : "Bilinmiyor",
                    TamamlanmaYuzdesi = a.HedefMiktar > 0
                        ? Math.Round(a.TamamlananMiktar / a.HedefMiktar * 100, 2)
                        : 0,
                    KalanSure = 0, // Basit hesaplama için 0, isterseniz tahmin yapabiliriz
                    BaslangicTarihi = a.BaslangicTarihi!.Value
                })
                .ToListAsync();
            var dusukStokUrunler = await _context.Urunler
                .Include(u => u.Stoklar)
                .Where(u => u.MinimumStok.HasValue)
                .ToListAsync();

            var dusukStokUyarilari = dusukStokUrunler
                .Where(u => u.Stoklar.Sum(s => s.Miktar) < u.MinimumStok)
                .Select(u => new DusukStokUyariDto
                {
                    UrunAdi = u.Ad,
                    UrunKodu = u.Kod,
                    MevcutStok = u.Stoklar.Sum(s => s.Miktar),
                    MinimumStok = u.MinimumStok!.Value,
                    EksikMiktar = u.MinimumStok!.Value - u.Stoklar.Sum(s => s.Miktar)
                })
                .OrderByDescending(u => u.EksikMiktar)
                .ToList();

            var ozet = new DashboardOzetDto
            {
                IsEmirleri = isEmirleriOzet,
                Istasyonlar = istasyonlarOzet,
                Uretim = uretimOzet,
                Hurda = hurdaOzet,
                AktifIsler = aktifIsler,
                DusukStokUyarilari = dusukStokUyarilari
            };

            return ResultDto<DashboardOzetDto>.SuccessResult(ozet);
        }
        catch (Exception ex)
        {
            return ResultDto<DashboardOzetDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<PerformansDto>>> GetPerformansAsync(DateTime baslangic, DateTime bitis)
    {
        try
        {
            var gunler = new List<PerformansDto>();

            for (var tarih = baslangic.Date; tarih <= bitis.Date; tarih = tarih.AddDays(1))
            {
                var gunSonu = tarih.AddDays(1);

                var tamamlananIsler = await _context.IsEmirleri
                    .Where(i => i.BitisTarihi.HasValue &&
                               i.BitisTarihi.Value >= tarih &&
                               i.BitisTarihi.Value < gunSonu)
                    .ToListAsync();

                var uretilenAdet = tamamlananIsler.Sum(i => (int)i.TamamlananMiktar);
                var hedefAdet = tamamlananIsler.Sum(i => (int)i.HedefMiktar);

                var hurdalar = await _context.Hurdalar
                    .Where(h => h.HurdaTarihi >= tarih && h.HurdaTarihi < gunSonu)
                    .ToListAsync();

                var hurdaAdet = hurdalar.Sum(h => (int)h.Miktar);
                var hurdaOrani = uretilenAdet + hurdaAdet > 0
                    ? Math.Round((decimal)hurdaAdet / (uretilenAdet + hurdaAdet) * 100, 2)
                    : 0;

                var verimlilik = hedefAdet > 0
                    ? Math.Round((decimal)uretilenAdet / hedefAdet * 100, 2)
                    : 0;
                var isAdimlari = await _context.IsAdimlari
                    .Where(a => a.BitisTarihi.HasValue &&
                               a.BitisTarihi.Value >= tarih &&
                               a.BitisTarihi.Value < gunSonu &&
                               a.BaslangicTarihi.HasValue)
                    .ToListAsync();

                var ortalamaCevrime = isAdimlari.Any()
                    ? Math.Round((decimal)isAdimlari
                        .Average(a => (a.BitisTarihi!.Value - a.BaslangicTarihi!.Value).TotalMinutes), 2)
                    : 0;

                gunler.Add(new PerformansDto
                {
                    Tarih = tarih,
                    Verimlilik = verimlilik,
                    UretilenAdet = uretilenAdet,
                    HedefAdet = hedefAdet,
                    HurdaAdet = hurdaAdet,
                    HurdaOrani = hurdaOrani,
                    OrtalamaCevrimeZamani = ortalamaCevrime
                });
            }

            return ResultDto<List<PerformansDto>>.SuccessResult(gunler);
        }
        catch (Exception ex)
        {
            return ResultDto<List<PerformansDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<CanliTakipDto>> GetCanliTakipAsync()
    {
        try
        {
            var istasyonlar = await _context.Istasyonlar
                .Include(i => i.AktifIsAdimi)
                    .ThenInclude(a => a!.IsEmri)
                        .ThenInclude(e => e.Urun)
                .Include(i => i.AktifIsAdimi)
                    .ThenInclude(a => a!.SorumluKullanici)
                .ToListAsync();

            var istasyonDurumlari = istasyonlar.Select(i => new IstasyonDurumDto
            {
                IstasyonId = i.Id,
                IstasyonAdi = i.Ad,
                IstasyonTipi = i.Tip.ToString(),
                Durum = i.Durum.ToString(),
                IsEmriNo = i.AktifIsAdimi?.IsEmri.IsEmriNo,
                UrunAdi = i.AktifIsAdimi?.IsEmri.Urun.Ad,
                TamamlanmaYuzdesi = i.AktifIsAdimi != null && i.AktifIsAdimi.HedefMiktar > 0
                    ? Math.Round(i.AktifIsAdimi.TamamlananMiktar / i.AktifIsAdimi.HedefMiktar * 100, 2)
                    : null,
                OperatorAdi = i.AktifIsAdimi?.SorumluKullanici != null
                    ? (i.AktifIsAdimi.SorumluKullanici.AdSoyad ?? i.AktifIsAdimi.SorumluKullanici.KullaniciAdi)
                    : null
            }).ToList();
            var sonHareketler = await _context.IsAdimiLoglari
                .Include(l => l.Kullanici)
                .OrderByDescending(l => l.IslemTarihi)
                .Take(10)
                .Select(l => new SonHareketDto
                {
                    Tip = l.YeniDurum.ToString(),
                    Mesaj = l.Aciklama ?? $"Durum deðiþti: {l.EskiDurum} › {l.YeniDurum}",
                    Zaman = l.IslemTarihi,
                    KullaniciAdi = l.Kullanici != null
                        ? (l.Kullanici.AdSoyad ?? l.Kullanici.KullaniciAdi)
                        : null
                })
                .ToListAsync();

            var canliTakip = new CanliTakipDto
            {
                IstasyonDurumlari = istasyonDurumlari,
                SonHareketler = sonHareketler,
                SonGuncelleme = DateTime.UtcNow
            };

            return ResultDto<CanliTakipDto>.SuccessResult(canliTakip);
        }
        catch (Exception ex)
        {
            return ResultDto<CanliTakipDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<IstasyonPerformansDto>>> GetIstasyonPerformansAsync(DateTime baslangic, DateTime bitis)
    {
        try
        {
            var istasyonlar = await _context.Istasyonlar.ToListAsync();
            var performansListesi = new List<IstasyonPerformansDto>();

            foreach (var istasyon in istasyonlar)
            {
                var isAdimlari = await _context.IsAdimlari
                    .Where(a => a.IstasyonId == istasyon.Id &&
                               a.BitisTarihi.HasValue &&
                               a.BitisTarihi.Value >= baslangic &&
                               a.BitisTarihi.Value <= bitis &&
                               a.BaslangicTarihi.HasValue)
                    .ToListAsync();

                var tamamlananIsSayisi = isAdimlari.Count;
                var toplamCalismaZamani = isAdimlari
                    .Sum(a => (a.BitisTarihi!.Value - a.BaslangicTarihi!.Value).TotalHours);

                var ortalamaCevrime = isAdimlari.Any()
                    ? isAdimlari.Average(a => (a.BitisTarihi!.Value - a.BaslangicTarihi!.Value).TotalMinutes)
                    : 0;

                var toplamHedef = isAdimlari.Sum(a => a.HedefMiktar);
                var toplamTamamlanan = isAdimlari.Sum(a => a.TamamlananMiktar);
                var verimlilik = toplamHedef > 0
                    ? Math.Round(toplamTamamlanan / toplamHedef * 100, 2)
                    : 0;

                performansListesi.Add(new IstasyonPerformansDto
                {
                    IstasyonAdi = istasyon.Ad,
                    TamamlananIsSayisi = tamamlananIsSayisi,
                    ToplamCalismaZamani = Math.Round((decimal)toplamCalismaZamani, 2),
                    OrtalamaCevrimeZamani = Math.Round((decimal)ortalamaCevrime, 2),
                    Verimlilik = verimlilik
                });
            }

            return ResultDto<List<IstasyonPerformansDto>>.SuccessResult(
                performansListesi.OrderByDescending(p => p.Verimlilik).ToList()
            );
        }
        catch (Exception ex)
        {
            return ResultDto<List<IstasyonPerformansDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }
}