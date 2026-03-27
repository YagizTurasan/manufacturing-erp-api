namespace BahadirMakina.Application.DTOs.IsAdimi;

public class AktifIsDetayDto
{
    public int IsAdimiId { get; set; }
    public string IsEmriNo { get; set; } = string.Empty;
    public string UrunAdi { get; set; } = string.Empty;
    public int Sira { get; set; }
    public string IstasyonAdi { get; set; } = string.Empty;
    public string IstasyonTipi { get; set; } = string.Empty;
    public string Tanim { get; set; } = string.Empty;
    public decimal HedefMiktar { get; set; }
    public decimal TamamlananMiktar { get; set; }
    public decimal KumulatifTamamlanan { get; set; }
    public decimal KalanMiktar { get; set; }
    public decimal HurdaMiktari { get; set; }
    public string Durum { get; set; } = string.Empty;
    public string BaslangisTarihi { get; set; } = string.Empty;
    public int GecenSure { get; set; } // Dakika
    public bool KaliteKontrolGerekli { get; set; }
    public int? SorumluKullaniciId { get; set; }
    public string? SorumluKullaniciAdi { get; set; }
}