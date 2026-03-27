using BahadirMakina.Domain.Enums;

namespace BahadirMakina.Domain.Entities;

public class Kullanici : BaseEntity
{
    public string KullaniciAdi { get; set; } = string.Empty; // tornaci1, kaynakci2
    public string? AdSoyad { get; set; } // Opsiyonel - ekranda gösterilmek için
    public string PasswordHash { get; set; } = string.Empty;
    public KullaniciRol Rol { get; set; }
    public bool Aktif { get; set; } = true; // Kullanýcý aktif mi?
    public ICollection<IsAdimiLog> IsAdimiLogs { get; set; } = new List<IsAdimiLog>();
    public ICollection<KaliteKontrol> KaliteKontroller { get; set; } = new List<KaliteKontrol>();
    public ICollection<DepoHareket> DepoHareketleri { get; set; } = new List<DepoHareket>();
}