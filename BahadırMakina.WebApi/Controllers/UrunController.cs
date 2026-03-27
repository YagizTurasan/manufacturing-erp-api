using BahadirMakina.Application.DTOs.Urun;
using BahadirMakina.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BahadirMakina.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UrunController : ControllerBase
{
    private readonly IUrunService _urunService;

    public UrunController(IUrunService urunService)
    {
        _urunService = urunService;
    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUrunDto dto)
    {
        var result = await _urunService.CreateAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _urunService.GetAllAsync();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _urunService.GetByIdAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
    [HttpGet("{id}/detay")]
    public async Task<IActionResult> GetDetay(int id)
    {
        var result = await _urunService.GetDetayByIdAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
    [HttpGet("tip/{tip}")]
    public async Task<IActionResult> GetByTip(string tip)
    {
        var result = await _urunService.GetByTipAsync(tip);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromBody] UpdateUrunDto dto)
    {
        var result = await _urunService.UpdateAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _urunService.DeleteAsync(id);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet("dusuk-stok")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetDusukStok()
    {
        var result = await _urunService.GetDusukStokUrunlerAsync();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPost("adim")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAdim([FromBody] CreateUrunAgaciAdimDto dto)
    {
        var result = await _urunService.CreateAdimAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetAdimlar), new { urunId = dto.UrunId }, result);
    }
    [HttpGet("{urunId}/adimlar")]
    public async Task<IActionResult> GetAdimlar(int urunId)
    {
        var result = await _urunService.GetAdimlarByUrunIdAsync(urunId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpDelete("adim/{adimId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAdim(int adimId)
    {
        var result = await _urunService.DeleteAdimAsync(adimId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPost("bilesen")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateBilesen([FromBody] CreateGerekliBilesenDto dto)
    {
        var result = await _urunService.CreateGerekliBilesenAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetBilesenler), new { urunId = dto.UrunId }, result);
    }
    [HttpGet("{urunId}/bilesenler")]
    public async Task<IActionResult> GetBilesenler(int urunId)
    {
        var result = await _urunService.GetGerekliBilesenlerByUrunIdAsync(urunId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpDelete("bilesen/{bilesenId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBilesen(int bilesenId)
    {
        var result = await _urunService.DeleteGerekliBilesenAsync(bilesenId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}