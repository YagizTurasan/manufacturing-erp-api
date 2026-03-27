namespace BahadirMakina.Application.DTOs.Kullanici;

public class KullaniciDto
{
    public int Id { get; set; }
    public string KullaniciAdi { get; set; } = string.Empty;
    public string? AdSoyad { get; set; }
    public string Rol { get; set; } = string.Empty;
    public bool Aktif { get; set; }
    public DateTime CreatedAt { get; set; }
}