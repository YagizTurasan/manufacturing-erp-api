using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.DTOs.IsAdimi;

namespace BahadirMakina.Application.Interfaces;

public interface IIsAdimiService
{
    Task<ResultDto<bool>> BaslatAsync(BaslatIsAdimiDto dto);
    Task<ResultDto<bool>> ParcaTamamlaAsync(ParcaTamamlaDto dto);
    Task<ResultDto<bool>> IslemiBitirAsync(int isAdimiId, int kullaniciId);
    Task<ResultDto<List<BekleyenIsDto>>> GetBekleyenIslerByIstasyonAsync(int istasyonId);
    Task<ResultDto<AktifIsDto>> GetAktifIsByIstasyonAsync(int istasyonId);

    Task<ResultDto<List<BekleyenIsDto>>> GetBekleyenIslerByKullaniciAsync(int kullaniciId);
    Task<ResultDto<AktifIsDetayDto>> GetAktifIsByKullaniciAsync(int kullaniciId);

}