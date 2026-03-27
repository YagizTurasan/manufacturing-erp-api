using BahadirMakina.Application.DTOs.KaliteKontrol;
using BahadirMakina.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BahadirMakina.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class KaliteKontrolController : ControllerBase
{
    private readonly IKaliteKontrolService _kaliteKontrolService;

    public KaliteKontrolController(IKaliteKontrolService kaliteKontrolService)
    {
        _kaliteKontrolService = kaliteKontrolService;
    }
    [HttpPost]
    [Authorize(Roles = "KaliteKontrol,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateKaliteKontrolDto dto)
    {
        var result = await _kaliteKontrolService.CreateKaliteKontrolAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
    }
    [HttpGet("bekleyen")]
    [Authorize(Roles = "KaliteKontrol,Admin")]
    public async Task<IActionResult> GetBekleyen()
    {
        var result = await _kaliteKontrolService.GetBekleyenKontrollerAsync();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet("{id}")]
    [Authorize(Roles = "KaliteKontrol,Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _kaliteKontrolService.GetByIdAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
    [HttpGet("is-adimi/{isAdimiId}")]
    [Authorize(Roles = "KaliteKontrol,Admin")]
    public async Task<IActionResult> GetByIsAdimiId(int isAdimiId)
    {
        var result = await _kaliteKontrolService.GetByIsAdimiIdAsync(isAdimiId);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}