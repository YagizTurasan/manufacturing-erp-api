using System.ComponentModel.DataAnnotations;

namespace BahadirMakina.Application.DTOs.Kullanici;

public class LoginDto
{
    [Required(ErrorMessage = "Kullanýcý adý zorunludur")]
    public string KullaniciAdi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Þifre zorunludur")]
    public string Sifre { get; set; } = string.Empty;
}