using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.DTOs.Kullanici;
using BahadirMakina.Application.Interfaces;
using BahadirMakina.Domain.Entities;
using BahadirMakina.Domain.Enums;
using BahadirMakina.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BahadirMakina.Application.Services;

public class KullaniciService : IKullaniciService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public KullaniciService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<ResultDto<LoginResponseDto>> LoginAsync(LoginDto dto)
    {
        try
        {
            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == dto.KullaniciAdi);

            if (kullanici == null)
                return ResultDto<LoginResponseDto>.FailureResult("Kullanýcý adý veya þifre hatalý!");
            if (!BCrypt.Net.BCrypt.Verify(dto.Sifre, kullanici.PasswordHash))
                return ResultDto<LoginResponseDto>.FailureResult("Kullanýcý adý veya þifre hatalý!");
            if (!kullanici.Aktif)
                return ResultDto<LoginResponseDto>.FailureResult("Hesabýnýz pasif durumda!");
            var token = GenerateJwtToken(kullanici);
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"]!);

            var response = new LoginResponseDto
            {
                Token = token,
                Kullanici = new KullaniciDto
                {
                    Id = kullanici.Id,
                    KullaniciAdi = kullanici.KullaniciAdi,
                    AdSoyad = kullanici.AdSoyad,
                    Rol = kullanici.Rol.ToString(),
                    Aktif = kullanici.Aktif,
                    CreatedAt = kullanici.CreatedAt
                },
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };

            return ResultDto<LoginResponseDto>.SuccessResult(response, "Giriþ baþarýlý!");
        }
        catch (Exception ex)
        {
            return ResultDto<LoginResponseDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<KullaniciDto>> GetByTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var kullaniciIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (kullaniciIdClaim == null)
                return ResultDto<KullaniciDto>.FailureResult("Geçersiz token!");

            var kullaniciId = int.Parse(kullaniciIdClaim.Value);
            return await GetByIdAsync(kullaniciId);
        }
        catch (Exception ex)
        {
            return ResultDto<KullaniciDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<KullaniciDto>> CreateAsync(CreateKullaniciDto dto)
    {
        try
        {
            var mevcutKullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == dto.KullaniciAdi);

            if (mevcutKullanici != null)
                return ResultDto<KullaniciDto>.FailureResult("Bu kullanýcý adý zaten kullanýlýyor!");
            if (!Enum.TryParse<KullaniciRol>(dto.Rol, out var rol))
                return ResultDto<KullaniciDto>.FailureResult("Geçersiz rol!");
            var kullanici = new Kullanici
            {
                KullaniciAdi = dto.KullaniciAdi,
                AdSoyad = dto.AdSoyad,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Sifre),
                Rol = rol,
                Aktif = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Kullanicilar.Add(kullanici);
            await _context.SaveChangesAsync();

            var responseDto = new KullaniciDto
            {
                Id = kullanici.Id,
                KullaniciAdi = kullanici.KullaniciAdi,
                AdSoyad = kullanici.AdSoyad,
                Rol = kullanici.Rol.ToString(),
                Aktif = kullanici.Aktif,
                CreatedAt = kullanici.CreatedAt
            };

            return ResultDto<KullaniciDto>.SuccessResult(responseDto, "Kullanýcý oluþturuldu!");
        }
        catch (Exception ex)
        {
            return ResultDto<KullaniciDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<KullaniciDto>> GetByIdAsync(int id)
    {
        try
        {
            var kullanici = await _context.Kullanicilar.FindAsync(id);

            if (kullanici == null)
                return ResultDto<KullaniciDto>.FailureResult("Kullanýcý bulunamadý!");

            var dto = new KullaniciDto
            {
                Id = kullanici.Id,
                KullaniciAdi = kullanici.KullaniciAdi,
                AdSoyad = kullanici.AdSoyad,
                Rol = kullanici.Rol.ToString(),
                Aktif = kullanici.Aktif,
                CreatedAt = kullanici.CreatedAt
            };

            return ResultDto<KullaniciDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            return ResultDto<KullaniciDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<KullaniciDto>>> GetAllAsync()
    {
        try
        {
            var kullanicilar = await _context.Kullanicilar
                .OrderBy(k => k.Rol)
                .ThenBy(k => k.AdSoyad)
                .ToListAsync();

            var dtoList = kullanicilar.Select(k => new KullaniciDto
            {
                Id = k.Id,
                KullaniciAdi = k.KullaniciAdi,
                AdSoyad = k.AdSoyad,
                Rol = k.Rol.ToString(),
                Aktif = k.Aktif,
                CreatedAt = k.CreatedAt
            }).ToList();

            return ResultDto<List<KullaniciDto>>.SuccessResult(dtoList);
        }
        catch (Exception ex)
        {
            return ResultDto<List<KullaniciDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<List<KullaniciDto>>> GetByRolAsync(string rol)
    {
        try
        {
            if (!Enum.TryParse<KullaniciRol>(rol, out var kullaniciRol))
                return ResultDto<List<KullaniciDto>>.FailureResult("Geçersiz rol!");

            var kullanicilar = await _context.Kullanicilar
                .Where(k => k.Rol == kullaniciRol)
                .OrderBy(k => k.AdSoyad)
                .ToListAsync();

            var dtoList = kullanicilar.Select(k => new KullaniciDto
            {
                Id = k.Id,
                KullaniciAdi = k.KullaniciAdi,
                AdSoyad = k.AdSoyad,
                Rol = k.Rol.ToString(),
                Aktif = k.Aktif,
                CreatedAt = k.CreatedAt
            }).ToList();

            return ResultDto<List<KullaniciDto>>.SuccessResult(dtoList);
        }
        catch (Exception ex)
        {
            return ResultDto<List<KullaniciDto>>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<KullaniciDto>> UpdateAsync(UpdateKullaniciDto dto)
    {
        try
        {
            var kullanici = await _context.Kullanicilar.FindAsync(dto.Id);

            if (kullanici == null)
                return ResultDto<KullaniciDto>.FailureResult("Kullanýcý bulunamadý!");
            if (!string.IsNullOrWhiteSpace(dto.AdSoyad))
                kullanici.AdSoyad = dto.AdSoyad;

            if (dto.Aktif.HasValue)
                kullanici.Aktif = dto.Aktif.Value;

            kullanici.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var responseDto = new KullaniciDto
            {
                Id = kullanici.Id,
                KullaniciAdi = kullanici.KullaniciAdi,
                AdSoyad = kullanici.AdSoyad,
                Rol = kullanici.Rol.ToString(),
                Aktif = kullanici.Aktif,
                CreatedAt = kullanici.CreatedAt
            };

            return ResultDto<KullaniciDto>.SuccessResult(responseDto, "Kullanýcý güncellendi!");
        }
        catch (Exception ex)
        {
            return ResultDto<KullaniciDto>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<bool>> DeleteAsync(int id)
    {
        try
        {
            var kullanici = await _context.Kullanicilar.FindAsync(id);

            if (kullanici == null)
                return ResultDto<bool>.FailureResult("Kullanýcý bulunamadý!");
            if (kullanici.Rol == KullaniciRol.Admin)
                return ResultDto<bool>.FailureResult("Admin kullanýcýsý silinemez!");
            kullanici.Aktif = false;
            kullanici.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ResultDto<bool>.SuccessResult(true, "Kullanýcý pasif edildi!");
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<bool>> ChangePasswordAsync(ChangePasswordDto dto)
    {
        try
        {
            var kullanici = await _context.Kullanicilar.FindAsync(dto.KullaniciId);

            if (kullanici == null)
                return ResultDto<bool>.FailureResult("Kullanýcý bulunamadý!");
            if (!BCrypt.Net.BCrypt.Verify(dto.EskiSifre, kullanici.PasswordHash))
                return ResultDto<bool>.FailureResult("Eski þifre hatalý!");
            kullanici.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.YeniSifre);
            kullanici.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ResultDto<bool>.SuccessResult(true, "Þifre deðiþtirildi!");
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.FailureResult($"Hata: {ex.Message}");
        }
    }

    public async Task<ResultDto<bool>> ResetPasswordAsync(int kullaniciId, string yeniSifre)
    {
        try
        {
            var kullanici = await _context.Kullanicilar.FindAsync(kullaniciId);

            if (kullanici == null)
                return ResultDto<bool>.FailureResult("Kullanýcý bulunamadý!");
            kullanici.PasswordHash = BCrypt.Net.BCrypt.HashPassword(yeniSifre);
            kullanici.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ResultDto<bool>.SuccessResult(true, "Þifre sýfýrlandý!");
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.FailureResult($"Hata: {ex.Message}");
        }
    }
    private string GenerateJwtToken(Kullanici kullanici)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!)
        );
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
            new Claim(ClaimTypes.Name, kullanici.KullaniciAdi),
            new Claim(ClaimTypes.GivenName, kullanici.AdSoyad ?? kullanici.KullaniciAdi),
            new Claim(ClaimTypes.Role, kullanici.Rol.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpirationInMinutes"]!)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}