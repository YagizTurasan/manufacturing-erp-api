using BahadirMakina.Application.DTOs.Kullanici;
using BahadirMakina.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BahadirMakina.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KullaniciController : ControllerBase
{
    private readonly IKullaniciService _kullaniciService;

    public KullaniciController(IKullaniciService kullaniciService)
    {
        _kullaniciService = kullaniciService;
    }
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _kullaniciService.LoginAsync(dto);

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var result = await _kullaniciService.GetByTokenAsync(token);

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateKullaniciDto dto)
    {
        var result = await _kullaniciService.CreateAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _kullaniciService.GetAllAsync();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _kullaniciService.GetByIdAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
    [HttpGet("rol/{rol}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByRol(string rol)
    {
        var result = await _kullaniciService.GetByRolAsync(rol);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromBody] UpdateKullaniciDto dto)
    {
        var result = await _kullaniciService.UpdateAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _kullaniciService.DeleteAsync(id);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var result = await _kullaniciService.ChangePasswordAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPost("reset-password")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResetPassword([FromQuery] int kullaniciId, [FromQuery] string yeniSifre)
    {
        var result = await _kullaniciService.ResetPasswordAsync(kullaniciId, yeniSifre);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}