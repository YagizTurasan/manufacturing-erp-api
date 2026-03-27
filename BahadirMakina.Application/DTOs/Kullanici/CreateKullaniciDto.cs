using System.ComponentModel.DataAnnotations;

namespace BahadirMakina.Application.DTOs.Kullanici;

public class CreateKullaniciDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string KullaniciAdi { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string AdSoyad { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 4)]
    public string Sifre { get; set; } = string.Empty;

    [Required]
    public string Rol { get; set; } = string.Empty;
}