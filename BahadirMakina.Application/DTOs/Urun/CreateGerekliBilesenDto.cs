using System.ComponentModel.DataAnnotations;

namespace BahadirMakina.Application.DTOs.Urun;

public class CreateGerekliBilesenDto
{
    [Required]
    public int UrunId { get; set; }

    [Required]
    public int BilesenUrunId { get; set; }

    [Required]
    public decimal Miktar { get; set; }
}