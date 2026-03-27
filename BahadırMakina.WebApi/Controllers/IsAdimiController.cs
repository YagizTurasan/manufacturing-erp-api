using BahadirMakina.Application.DTOs.IsAdimi;
using BahadirMakina.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BahadirMakina.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IsAdimiController : ControllerBase
{
    private readonly IIsAdimiService _isAdimiService;

    public IsAdimiController(IIsAdimiService isAdimiService)
    {
        _isAdimiService = isAdimiService;
    }
    [HttpPost("baslat")]
    [Authorize(Roles = "Admin,Tornaci,Kaynakci,Kumlamaci,Presci,Abkantci,Lazerci,Taslamaci,DogrultmaOperatoru, DikIslemci")]
    public async Task<IActionResult> Baslat([FromBody] BaslatIsAdimiDto dto)
    {
        var result = await _isAdimiService.BaslatAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPost("parca-tamamla")]
    public async Task<IActionResult> ParcaTamamla([FromBody] ParcaTamamlaDto dto)
    {
        var result = await _isAdimiService.ParcaTamamlaAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPost("{isAdimiId}/bitir")]
    [Authorize(Roles = "Admin,Tornaci,Kaynakci,Kumlamaci,Presci,Abkantci,Lazerci,Taslamaci,DogrultmaOperatoru, DikIslemci")]
    public async Task<IActionResult> IslemiBitir(int isAdimiId, [FromQuery] int kullaniciId)
    {
        var result = await _isAdimiService.IslemiBitirAsync(isAdimiId, kullaniciId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet("istasyon/{istasyonId}/bekleyen-isler")]
    public async Task<IActionResult> GetBekleyenIsler(int istasyonId)
    {
        var result = await _isAdimiService.GetBekleyenIslerByIstasyonAsync(istasyonId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet("istasyon/{istasyonId}/aktif-is")]
    public async Task<IActionResult> GetAktifIs(int istasyonId)
    {
        var result = await _isAdimiService.GetAktifIsByIstasyonAsync(istasyonId);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
    [HttpGet("kullanici/{kullaniciId}/bekleyen-isler")]
    public async Task<IActionResult> GetBekleyenIslerByKullanici(int kullaniciId)
    {
        var result = await _isAdimiService.GetBekleyenIslerByKullaniciAsync(kullaniciId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet("kullanici/{kullaniciId}/aktif-is")]
    public async Task<IActionResult> GetAktifIsByKullanici(int kullaniciId)
    {
        var result = await _isAdimiService.GetAktifIsByKullaniciAsync(kullaniciId);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}