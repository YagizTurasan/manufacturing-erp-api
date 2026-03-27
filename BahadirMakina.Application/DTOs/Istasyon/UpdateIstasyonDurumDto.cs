namespace BahadirMakina.Application.DTOs.Istasyon;

public class UpdateIstasyonDurumDto
{
    public int IstasyonId { get; set; }
    public string Durum { get; set; } = string.Empty; // Musait, Mesgul, Arizali, Bakimda
    public string? Aciklama { get; set; }
    public int KullaniciId { get; set; }
}