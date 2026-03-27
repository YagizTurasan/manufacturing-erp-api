using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.DTOs.IsAdimi;
using BahadirMakina.Application.DTOs.Istasyon;
using BahadirMakina.Domain.Enums;

namespace BahadirMakina.Application.Interfaces;

public interface IIstasyonService
{
    Task<ResultDto<IstasyonDto>> GetByIdAsync(int id);
    Task<ResultDto<List<IstasyonDto>>> GetAllAsync();
    Task<ResultDto<List<IstasyonDto>>> GetByTipAsync(IstasyonTip tip);
    Task<ResultDto<List<IstasyonDto>>> GetByDurumAsync(IstasyonDurum durum);
    Task<ResultDto<IstasyonDetayDto>> GetDetayByIdAsync(int id);
    Task<ResultDto<bool>> UpdateDurumAsync(UpdateIstasyonDurumDto dto);
    Task<ResultDto<List<IstasyonDto>>> GetDashboardOzetAsync();
}