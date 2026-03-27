namespace BahadirMakina.Application.DTOs.Stok;

public class StokDto
{
    public int Id { get; set; }
    public string UrunAdi { get; set; } = string.Empty;
    public string UrunKodu { get; set; } = string.Empty;
    public string DepoAdi { get; set; } = string.Empty;
    public decimal Miktar { get; set; }
    public string Birim { get; set; } = string.Empty;
    public string Durum { get; set; } = string.Empty;
    public decimal? MinimumStok { get; set; }
    public bool DusukStok { get; set; }
}

public class StokGirisiDto
{
    public int UrunId { get; set; }
    public int DepoId { get; set; }
    public decimal Miktar { get; set; }
    public string? Aciklama { get; set; }
    public int? KullaniciId { get; set; } // ? EKLE
}

public class StokCikisiDto
{
    public int UrunId { get; set; }
    public int DepoId { get; set; }
    public decimal Miktar { get; set; }
    public string? Aciklama { get; set; }
    public int? KullaniciId { get; set; } // ? EKLE
}