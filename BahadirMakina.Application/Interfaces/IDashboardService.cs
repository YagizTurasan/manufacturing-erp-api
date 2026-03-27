using BahadirMakina.Application.DTOs.Common;
using BahadirMakina.Application.DTOs.Dashboard;

namespace BahadirMakina.Application.Interfaces;

public interface IDashboardService
{
    Task<ResultDto<DashboardOzetDto>> GetOzetAsync();
    Task<ResultDto<List<PerformansDto>>> GetPerformansAsync(DateTime baslangic, DateTime bitis);
    Task<ResultDto<CanliTakipDto>> GetCanliTakipAsync();
    Task<ResultDto<List<IstasyonPerformansDto>>> GetIstasyonPerformansAsync(DateTime baslangic, DateTime bitis);
}

public class IstasyonPerformansDto
{
    public string IstasyonAdi { get; set; } = string.Empty;
    public int TamamlananIsSayisi { get; set; }
    public decimal ToplamCalismaZamani { get; set; } // Saat
    public decimal OrtalamaCevrimeZamani { get; set; } // Dakika
    public decimal Verimlilik { get; set; } // %
}