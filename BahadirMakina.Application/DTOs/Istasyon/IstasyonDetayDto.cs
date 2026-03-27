using BahadirMakina.Application.DTOs.IsAdimi;

namespace BahadirMakina.Application.DTOs.Istasyon;

public class IstasyonDetayDto
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Kod { get; set; } = string.Empty;
    public string Tip { get; set; } = string.Empty;
    public string Durum { get; set; } = string.Empty;
    public string DepoAdi { get; set; } = string.Empty;
    public AktifIsDto? AktifIs { get; set; }
    public List<BekleyenIsDto> BekleyenIsler { get; set; } = new();
    public IstatistikDto Istatistikler { get; set; } = new();
}