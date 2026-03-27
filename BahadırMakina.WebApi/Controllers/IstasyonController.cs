using BahadirMakina.Application.DTOs.Istasyon;
using BahadirMakina.Application.Interfaces;
using BahadirMakina.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BahadirMakina.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IstasyonController : ControllerBase
{
    private readonly IIstasyonService _istasyonService;

    public IstasyonController(IIstasyonService istasyonService)
    {
        _istasyonService = istasyonService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
        {
        var result = await _istasyonService.GetAllAsync();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _istasyonService.GetByIdAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
    [HttpGet("{id}/detay")]
    public async Task<IActionResult> GetDetay(int id)
    {
        var result = await _istasyonService.GetDetayByIdAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
    [HttpGet("tip/{tip}")]
    public async Task<IActionResult> GetByTip(string tip)
    {
        if (!Enum.TryParse<IstasyonTip>(tip, out var istasyonTip))
            return BadRequest("Geçersiz istasyon tipi!");

        var result = await _istasyonService.GetByTipAsync(istasyonTip);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet("durum/{durum}")]
    public async Task<IActionResult> GetByDurum(string durum)
    {
        if (!Enum.TryParse<IstasyonDurum>(durum, out var istasyonDurum))
            return BadRequest("Geçersiz istasyon durumu!");

        var result = await _istasyonService.GetByDurumAsync(istasyonDurum);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet("dashboard-ozet")]
    public async Task<IActionResult> GetDashboardOzet()
    {
        var result = await _istasyonService.GetDashboardOzetAsync();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPost("durum-guncelle")]
    public async Task<IActionResult> UpdateDurum([FromBody] UpdateIstasyonDurumDto dto)
    {
        var result = await _istasyonService.UpdateDurumAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}