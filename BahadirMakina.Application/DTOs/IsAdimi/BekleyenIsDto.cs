namespace BahadirMakina.Application.DTOs.IsAdimi;

public class BekleyenIsDto
{
    public int IsAdimiId { get; set; } // ? Mevcut
    public string IsEmriNo { get; set; } = string.Empty; // ? Mevcut
    public string UrunAdi { get; set; } = string.Empty; // ? Mevcut
    public string UrunKodu { get; set; } = string.Empty; // ? EKLE
    public decimal HedefMiktar { get; set; } // ? Mevcut
    public int IstasyonId { get; set; }
    public int Sira { get; set; } // ? EKLE
    public string IstasyonTipi { get; set; } = string.Empty; // ? EKLE
    public string Tanim { get; set; } = string.Empty; // ? Mevcut
    public string Oncelik { get; set; } = "Orta"; // ? EKLE
    public string CreatedAt { get; set; } = string.Empty; // ? EKLE
    public List<GerekliBilesenDto>? GerekliBilesenler { get; set; } // ? Mevcut
}

public class GerekliBilesenDto
{
    public string UrunAdi { get; set; } = string.Empty;
    public decimal Miktar { get; set; }
    public bool StokYeterli { get; set; }
}