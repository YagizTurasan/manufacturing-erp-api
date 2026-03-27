namespace BahadirMakina.Application.DTOs.KaliteKontrol;

public class KaliteKontrolDto
{
    public int Id { get; set; }
    public int IsAdimiId { get; set; }
    public string IsEmriNo { get; set; } = string.Empty;
    public string UrunAdi { get; set; } = string.Empty;
    public string DepoAdi { get; set; } = string.Empty;
    public decimal KontrolEdilenMiktar { get; set; }
    public decimal OnaylananMiktar { get; set; }
    public decimal RedMiktar { get; set; }
    public string Sonuc { get; set; } = string.Empty;
    public string? RedSebebi { get; set; }
    public string KontrolEdenKullanici { get; set; } = string.Empty;
    public DateTime KontrolTarihi { get; set; }
}