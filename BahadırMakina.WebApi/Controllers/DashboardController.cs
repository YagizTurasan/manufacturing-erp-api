using BahadirMakina.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BahadirMakina.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }
    [HttpGet("ozet")]
    public async Task<IActionResult> GetOzet()
    {
        var result = await _dashboardService.GetOzetAsync();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet("performans")]
    public async Task<IActionResult> GetPerformans(
        [FromQuery] DateTime? baslangic = null,
        [FromQuery] DateTime? bitis = null)
    {
        var baslangicTarihi = baslangic ?? DateTime.Today.AddDays(-7);
        var bitisTarihi = bitis ?? DateTime.Today;

        var result = await _dashboardService.GetPerformansAsync(baslangicTarihi, bitisTarihi);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet("canli-takip")]
    public async Task<IActionResult> GetCanliTakip()
    {
        var result = await _dashboardService.GetCanliTakipAsync();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet("istasyon-performans")]
    public async Task<IActionResult> GetIstasyonPerformans(
        [FromQuery] DateTime? baslangic = null,
        [FromQuery] DateTime? bitis = null)
    {
        var baslangicTarihi = baslangic ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var bitisTarihi = bitis ?? DateTime.Today;

        var result = await _dashboardService.GetIstasyonPerformansAsync(baslangicTarihi, bitisTarihi);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}