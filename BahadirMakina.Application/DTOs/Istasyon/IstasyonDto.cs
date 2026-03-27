namespace BahadirMakina.Application.DTOs.Istasyon;

public class IstasyonDto
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Kod { get; set; } = string.Empty;
    public string Tip { get; set; } = string.Empty;
    public string Durum { get; set; } = string.Empty;
    public string DepoAdi { get; set; } = string.Empty;
    public bool AktifIsVar { get; set; }
    public int BekleyenIsSayisi { get; set; }
}