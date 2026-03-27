using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.DTOs.IsEmri;

namespace BahadirMakina.Application.Interfaces;

public interface IIsEmriService
{
    Task<ResultDto<IsEmriDto>> CreateIsEmriAsync(CreateIsEmriDto dto, int olusturanKullaniciId);
    Task<ResultDto<IsEmriDto>> GetByIdAsync(int id);
    Task<ResultDto<List<IsEmriDto>>> GetAllAsync();
    Task<ResultDto<List<IsEmriDto>>> GetByDurumAsync(string durum);
    Task<ResultDto<bool>> CancelIsEmriAsync(int id, int kullaniciId);
}