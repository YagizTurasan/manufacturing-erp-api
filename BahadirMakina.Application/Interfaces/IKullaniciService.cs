using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.DTOs.Kullanici;

namespace BahadirMakina.Application.Interfaces;

public interface IKullaniciService
{
    Task<ResultDto<LoginResponseDto>> LoginAsync(LoginDto dto);
    Task<ResultDto<KullaniciDto>> GetByTokenAsync(string token);
    Task<ResultDto<KullaniciDto>> CreateAsync(CreateKullaniciDto dto);
    Task<ResultDto<KullaniciDto>> GetByIdAsync(int id);
    Task<ResultDto<List<KullaniciDto>>> GetAllAsync();
    Task<ResultDto<List<KullaniciDto>>> GetByRolAsync(string rol);
    Task<ResultDto<KullaniciDto>> UpdateAsync(UpdateKullaniciDto dto);
    Task<ResultDto<bool>> DeleteAsync(int id);
    Task<ResultDto<bool>> ChangePasswordAsync(ChangePasswordDto dto);
    Task<ResultDto<bool>> ResetPasswordAsync(int kullaniciId, string yeniSifre);
}