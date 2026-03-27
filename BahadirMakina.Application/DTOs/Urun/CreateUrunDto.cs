using System.ComponentModel.DataAnnotations;

namespace BahadirMakina.Application.DTOs.Urun;

public class CreateUrunDto
{
    [Required]
    [StringLength(200)]
    public string Ad { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Kod { get; set; } = string.Empty;

    [Required]
    public string Tip { get; set; } = string.Empty; // Hammadde, YariMamul, Mamul

    [Required]
    [StringLength(20)]
    public string Birim { get; set; } = string.Empty;

    public decimal? MinimumStok { get; set; }
}