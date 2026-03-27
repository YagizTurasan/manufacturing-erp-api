using System.ComponentModel.DataAnnotations;

namespace BahadirMakina.Application.DTOs.Urun;

public class UpdateUrunDto
{
    public int Id { get; set; }

    [StringLength(200)]
    public string? Ad { get; set; }

    [StringLength(50)]
    public string? Kod { get; set; }

    [StringLength(20)]
    public string? Birim { get; set; }

    public decimal? MinimumStok { get; set; }
}