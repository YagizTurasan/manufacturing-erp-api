using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.DTOs.KaliteKontrol;

namespace BahadirMakina.Application.Interfaces;

public interface IKaliteKontrolService
{
    Task<ResultDto<int>> CreateKaliteKontrolAsync(CreateKaliteKontrolDto dto);
    Task<ResultDto<List<BekleyenKaliteKontrolDto>>> GetBekleyenKontrollerAsync();
    Task<ResultDto<KaliteKontrolDto>> GetByIdAsync(int id);
    Task<ResultDto<KaliteKontrolDto>> GetByIsAdimiIdAsync(int isAdimiId);
}