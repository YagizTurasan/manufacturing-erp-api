using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.DTOs.Urun;

namespace BahadirMakina.Application.Interfaces;

public interface IUrunService
{
    Task<ResultDto<UrunDto>> CreateAsync(CreateUrunDto dto);
    Task<ResultDto<UrunDto>> GetByIdAsync(int id);
    Task<ResultDto<UrunDetayDto>> GetDetayByIdAsync(int id);
    Task<ResultDto<List<UrunDto>>> GetAllAsync();
    Task<ResultDto<List<UrunDto>>> GetByTipAsync(string tip);
    Task<ResultDto<UrunDto>> UpdateAsync(UpdateUrunDto dto);
    Task<ResultDto<bool>> DeleteAsync(int id);
    Task<ResultDto<UrunAgaciAdimDto>> CreateAdimAsync(CreateUrunAgaciAdimDto dto);
    Task<ResultDto<bool>> DeleteAdimAsync(int adimId);
    Task<ResultDto<List<UrunAgaciAdimDto>>> GetAdimlarByUrunIdAsync(int urunId);
    Task<ResultDto<GerekliBilesenDetayDto>> CreateGerekliBilesenAsync(CreateGerekliBilesenDto dto);
    Task<ResultDto<bool>> DeleteGerekliBilesenAsync(int bilesenId);
    Task<ResultDto<List<GerekliBilesenDetayDto>>> GetGerekliBilesenlerByUrunIdAsync(int urunId);
    Task<ResultDto<List<UrunDto>>> GetDusukStokUrunlerAsync();
}