namespace BahadirMakina.Application.DTOs.IsAdimi;

public class AktifIsDto
{
    public int IsAdimiId { get; set; }
    public string IsEmriNo { get; set; } = string.Empty;
    public string UrunAdi { get; set; } = string.Empty;
    public string Tanim { get; set; } = string.Empty;
    public decimal HedefMiktar { get; set; }
    public decimal TamamlananMiktar { get; set; }
    public DateTime BaslangicTarihi { get; set; }
    public TimeSpan GecenSure { get; set; }
    public string SorumluKullanici { get; set; } = string.Empty;
}