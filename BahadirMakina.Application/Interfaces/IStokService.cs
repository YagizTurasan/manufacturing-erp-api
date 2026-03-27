using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.DTOs.Stok;

namespace BahadirMakina.Application.Interfaces;

public interface IStokService
{
    Task<ResultDto<List<StokDto>>> GetAllAsync();
    Task<ResultDto<List<StokDto>>> GetByDepoIdAsync(int depoId);
    Task<ResultDto<List<StokDto>>> GetByUrunIdAsync(int urunId);
    Task<ResultDto<StokDto>> StokGirisiAsync(StokGirisiDto dto);
    Task<ResultDto<StokDto>> StokCikisiAsync(StokCikisiDto dto);
    Task<ResultDto<List<StokDto>>> GetDusukStoklarAsync();
    Task<ResultDto<bool>> StokTransferAsync(StokTransferDto dto);
    Task<ResultDto<bool>> StokSayimAsync(StokSayimDto dto);

}