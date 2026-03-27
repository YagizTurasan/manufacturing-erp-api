using System.ComponentModel.DataAnnotations;

namespace BahadirMakina.Application.DTOs.Kullanici;

public class ChangePasswordDto
{
    public int KullaniciId { get; set; }

    [Required]
    public string EskiSifre { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 4)]
    public string YeniSifre { get; set; } = string.Empty;

    [Required]
    [Compare("YeniSifre", ErrorMessage = "Þifreler eþleþmiyor")]
    public string YeniSifreTekrar { get; set; } = string.Empty;
}