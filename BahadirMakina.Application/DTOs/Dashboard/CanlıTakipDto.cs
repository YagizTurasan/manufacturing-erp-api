namespace BahadirMakina.Application.DTOs.Dashboard;

public class CanliTakipDto
{
    public List<IstasyonDurumDto> IstasyonDurumlari { get; set; } = new();
    public List<SonHareketDto> SonHareketler { get; set; } = new();
    public DateTime SonGuncelleme { get; set; }
}

public class IstasyonDurumDto
{
    public int IstasyonId { get; set; }
    public string IstasyonAdi { get; set; } = string.Empty;
    public string IstasyonTipi { get; set; } = string.Empty;
    public string Durum { get; set; } = string.Empty;
    public string? IsEmriNo { get; set; }
    public string? UrunAdi { get; set; }
    public decimal? TamamlanmaYuzdesi { get; set; }
    public string? OperatorAdi { get; set; }
}

public class SonHareketDto
{
    public string Tip { get; set; } = string.Empty;
    public string Mesaj { get; set; } = string.Empty;
    public DateTime Zaman { get; set; }
    public string? KullaniciAdi { get; set; }
}