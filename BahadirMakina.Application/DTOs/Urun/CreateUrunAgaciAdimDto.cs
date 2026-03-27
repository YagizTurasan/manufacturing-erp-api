using System.ComponentModel.DataAnnotations;

namespace BahadirMakina.Application.DTOs.Urun;

public class CreateUrunAgaciAdimDto
{
    [Required]
    public int UrunId { get; set; }

    [Required]
    public int Sira { get; set; }

    [Required]
    public string IstasyonTipi { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Tanim { get; set; } = string.Empty;

    public int? CiktiUrunId { get; set; }

    public bool KaliteKontrolGerekli { get; set; }

    [Required]
    public int HedefDepoId { get; set; }
}