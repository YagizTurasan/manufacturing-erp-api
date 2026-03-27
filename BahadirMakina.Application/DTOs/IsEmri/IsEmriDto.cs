namespace BahadirMakina.Application.DTOs.IsEmri;

public class IsEmriDto
{
    public int Id { get; set; }
    public string IsEmriNo { get; set; } = string.Empty;
    public int UrunId { get; set; }
    public string UrunAdi { get; set; } = string.Empty;
    public decimal HedefMiktar { get; set; }
    public decimal TamamlananMiktar { get; set; }
    public decimal HurdaMiktar { get; set; }
    public string Durum { get; set; } = string.Empty;
    public DateTime? BaslangicTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
    public string OlusturanKullanici { get; set; } = string.Empty;
    public string? Notlar { get; set; }
    public List<IsAdimiDto>? IsAdimlari { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class IsAdimiDto
{
    public int Id { get; set; }
    public int Sira { get; set; }
    public string IstasyonAdi { get; set; } = string.Empty;
    public string Tanim { get; set; } = string.Empty;
    public string Durum { get; set; } = string.Empty;
    public decimal HedefMiktar { get; set; }
    public decimal TamamlananMiktar { get; set; }
    public DateTime? BaslangicTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
}