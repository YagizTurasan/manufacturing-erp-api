using BahadirMakina.Application.Interfaces;
using BahadirMakina.Domain.Enums;
using BahadirMakina.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BahadirMakina.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepoController : ControllerBase
{
    private readonly IDepoService _depoService;
    private readonly ApplicationDbContext _context;

    public DepoController(IDepoService depoService, ApplicationDbContext context)
    {
        _depoService = depoService;
        _context = context;
    }
    [HttpGet("stok-kontrol")]
    public async Task<IActionResult> StokKontrol(
        [FromQuery] int depoId,
        [FromQuery] int urunId,
        [FromQuery] decimal gerekliMiktar)
    {
        var result = await _depoService.StokKontrolAsync(depoId, urunId, gerekliMiktar);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllDepolar()
    {
        var depolar = await _context.Depolar
            .Select(d => new
            {
                d.Id,
                d.Ad,
                d.Kod,
                Tip = d.Tip.ToString(),
                d.Aciklama
            })
            .ToListAsync();

        return Ok(new { success = true, data = depolar });
    }
    [HttpGet("{depoId}/stoklar")]
    public async Task<IActionResult> GetStoklarByDepo(int depoId)
    {
        var stoklar = await _context.Stoklar
            .Include(s => s.Urun)
            .Include(s => s.Depo)
            .Include(s => s.IsEmri)
            .Include(s => s.IsAdimi)
            .Where(s => s.DepoId == depoId)
            .Select(s => new
            {
                s.Id,
                DepoAdi = s.Depo.Ad,
                UrunAdi = s.Urun.Ad,
                UrunKodu = s.Urun.Kod,
                s.Miktar,
                Durum = s.Durum.ToString(),
                IsEmriNo = s.IsEmri != null ? s.IsEmri.IsEmriNo : null,
                IsAdimiSira = s.IsAdimi != null ? s.IsAdimi.Sira : (int?)null,
                s.CreatedAt
            })
            .OrderBy(s => s.UrunAdi)
            .ToListAsync();

        return Ok(new { success = true, data = stoklar });
    }
    [HttpGet("urun/{urunId}/stoklar")]
    public async Task<IActionResult> GetStoklarByUrun(int urunId)
    {
        var stoklar = await _context.Stoklar
            .Include(s => s.Urun)
            .Include(s => s.Depo)
            .Include(s => s.IsEmri)
            .Where(s => s.UrunId == urunId)
            .Select(s => new
            {
                s.Id,
                DepoAdi = s.Depo.Ad,
                DepoTip = s.Depo.Tip.ToString(),
                s.Miktar,
                Durum = s.Durum.ToString(),
                IsEmriNo = s.IsEmri != null ? s.IsEmri.IsEmriNo : null,
                s.CreatedAt
            })
            .OrderBy(s => s.DepoAdi)
            .ToListAsync();

        var urun = await _context.Urunler.FindAsync(urunId);

        return Ok(new
        {
            success = true,
            data = new
            {
                urunAdi = urun?.Ad,
                urunKodu = urun?.Kod,
                stoklar = stoklar,
                toplamMiktar = stoklar.Sum(s => s.Miktar)
            }
        });
    }
    [HttpGet("stoklar")]
    public async Task<IActionResult> GetAllStoklar()
    {
        var stoklar = await _context.Stoklar
            .Include(s => s.Urun)
            .Include(s => s.Depo)
            .Where(s => s.Miktar > 0)
            .GroupBy(s => new { s.UrunId, s.Urun.Ad, s.Urun.Kod })
            .Select(g => new
            {
                UrunId = g.Key.UrunId,
                UrunAdi = g.Key.Ad,
                UrunKodu = g.Key.Kod,
                ToplamMiktar = g.Sum(s => s.Miktar),
                Depolar = g.Select(s => new
                {
                    DepoAdi = s.Depo.Ad,
                    Miktar = s.Miktar,
                    Durum = s.Durum.ToString()
                }).ToList()
            })
            .OrderBy(s => s.UrunAdi)
            .ToListAsync();

        return Ok(new { success = true, data = stoklar });
    }
    [HttpGet("hareketler")]
    public async Task<IActionResult> GetDepoHareketleri(
        [FromQuery] int? depoId = null,
        [FromQuery] int? isEmriId = null,
        [FromQuery] int? limit = 50)
    {
        var query = _context.DepoHareketleri
            .Include(h => h.Urun)
            .Include(h => h.KaynakDepo)
            .Include(h => h.HedefDepo)
            .Include(h => h.Istasyon)
            .Include(h => h.IsEmri)
            .Include(h => h.Kullanici)
            .AsQueryable();

        if (depoId.HasValue)
            query = query.Where(h => h.KaynakDepoId == depoId || h.HedefDepoId == depoId);

        if (isEmriId.HasValue)
            query = query.Where(h => h.IsEmriId == isEmriId);

        var hareketler = await query
            .OrderByDescending(h => h.IslemTarihi)
            .Take(limit.Value)
            .Select(h => new
            {
                h.Id,
                IsEmriNo = h.IsEmri != null ? h.IsEmri.IsEmriNo : null,
                UrunAdi = h.Urun.Ad,
                KaynakDepoAdi = h.KaynakDepo != null ? h.KaynakDepo.Ad : "Yok",
                HedefDepoAdi = h.HedefDepo != null ? h.HedefDepo.Ad : "Yok",
                h.Miktar,
                Tip = h.Tip.ToString(),
                IstasyonAdi = h.Istasyon != null ? h.Istasyon.Ad : null,
                KullaniciAdi = h.Kullanici != null ? h.Kullanici.AdSoyad : null,
                h.Aciklama,
                h.IslemTarihi
            })
            .ToListAsync();

        return Ok(new { success = true, data = hareketler });
    }
    [HttpGet("hurda")]
    public async Task<IActionResult> GetHurdalar()
    {
        var hurdalar = await _context.Hurdalar
            .Include(h => h.Urun)
            .Include(h => h.IsEmri)
            .Include(h => h.IsAdimi)
            .Include(h => h.KaynakDepo)
            .OrderByDescending(h => h.HurdaTarihi)
            .Select(h => new
            {
                h.Id,
                IsEmriNo = h.IsEmri.IsEmriNo,
                UrunAdi = h.Urun.Ad,
                h.Miktar,
                h.Sebep,
                KaynakDepoAdi = h.KaynakDepo.Ad,
                h.TahminiMaliyetKaybi,
                h.HurdaTarihi
            })
            .ToListAsync();

        var toplamMaliyet = hurdalar.Sum(h => h.TahminiMaliyetKaybi ?? 0);

        return Ok(new
        {
            success = true,
            data = new
            {
                hurdalar = hurdalar,
                toplamHurdaAdedi = hurdalar.Sum(h => h.Miktar),
                toplamMaliyetKaybi = toplamMaliyet
            }
        });
    }
    [HttpGet("istasyon-tipi/{istasyonTipi}/varsayilan-depo")]
    public async Task<IActionResult> GetVarsayilanDepoByIstasyonTipi(string istasyonTipi)
    {
        if (!Enum.TryParse<IstasyonTip>(istasyonTipi, out var tip))
            return BadRequest(new { success = false, message = "Geçersiz istasyon tipi!" });
        var depoTipi = tip switch
        {
            IstasyonTip.Torna => DepoTip.TalasliImalat,
            IstasyonTip.DikIslem => DepoTip.TalasliImalat,
            IstasyonTip.GazaltiKaynak => DepoTip.Kaynak,
            IstasyonTip.RobotKaynak => DepoTip.Kaynak,
            IstasyonTip.HidrolikPres => DepoTip.Pres,
            IstasyonTip.Abkant => DepoTip.Abkant,
            IstasyonTip.Lazer => DepoTip.Lazer,
            IstasyonTip.Taslama => DepoTip.Taslama,
            IstasyonTip.Kumlama => DepoTip.Kumlama,
            IstasyonTip.Dogrultma => DepoTip.Dogrultma,
            _ => DepoTip.YariMamul
        };

        var depo = await _context.Depolar
            .FirstOrDefaultAsync(d => d.Tip == depoTipi);

        if (depo == null)
            return NotFound(new { success = false, message = "Varsayýlan depo bulunamadý!" });

        return Ok(new
        {
            success = true,
            data = new
            {
                depoId = depo.Id,
                depoAdi = depo.Ad,
                depoKodu = depo.Kod,
                depoTipi = depo.Tip.ToString()
            }
        });
    }
}