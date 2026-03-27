using BahadirMakina.Application.DTOs.IsEmri;
using BahadirMakina.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BahadirMakina.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IsEmriController : ControllerBase
{
    private readonly IIsEmriService _isEmriService;

    public IsEmriController(IIsEmriService isEmriService)
    {
        _isEmriService = isEmriService;
    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateIsEmriDto dto, [FromQuery] int olusturanKullaniciId)
    {
        var result = await _isEmriService.CreateIsEmriAsync(dto, olusturanKullaniciId);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _isEmriService.GetAllAsync();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _isEmriService.GetByIdAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
    [HttpGet("durum/{durum}")]
    public async Task<IActionResult> GetByDurum(string durum)
    {
        var result = await _isEmriService.GetByDurumAsync(durum);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPost("{id}/iptal")]
    public async Task<IActionResult> Cancel(int id, [FromQuery] int kullaniciId)
    {
        var result = await _isEmriService.CancelIsEmriAsync(id, kullaniciId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}