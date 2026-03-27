using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Domain.Enums;

namespace BahadirMakina.Application.Interfaces;

public interface IDepoService
{
    Task<ResultDto<bool>> StokKontrolAsync(int depoId, int urunId, decimal gerekliMiktar);
    Task<ResultDto<bool>> RezervasyonYapAsync(int depoId, int urunId, decimal miktar, int isEmriId, int isAdimiId);
    Task<ResultDto<bool>> RezervasyonuIptalEtAsync(int depoId, int urunId, int isEmriId);
    Task<ResultDto<int>> CreateDepoHareketAsync(
        int? isEmriId,
        int urunId,
        int? kaynakDepoId,
        int? hedefDepoId,
        decimal miktar,
        DepoHareketTip tip,
        int? istasyonId,
        int? isAdimiId,
        int? kullaniciId,
        string? aciklama
    );
    Task<ResultDto<bool>> StokGuncelleAsync(int depoId, int urunId, decimal miktar, StokDurum durum);
    Task<ResultDto<bool>> StokOlusturVeyaGuncelleAsync(
    int? depoId,
    int urunId,
    decimal miktar,
    StokDurum durum,
    int? isEmriId = null,
    int? isAdimiId = null);
}