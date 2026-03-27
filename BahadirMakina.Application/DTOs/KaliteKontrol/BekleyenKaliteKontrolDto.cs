namespace BahadirMakina.Application.DTOs.KaliteKontrol;

public class BekleyenKaliteKontrolDto
{
    public int IsAdimiId { get; set; }
    public string IsEmriNo { get; set; } = string.Empty;
    public string UrunAdi { get; set; } = string.Empty;
    public string DepoAdi { get; set; } = string.Empty;
    public decimal Miktar { get; set; }
    public string IstasyonAdi { get; set; } = string.Empty;
    public string SorumluOperator { get; set; } = string.Empty;
    public DateTime BitisTarihi { get; set; }
}