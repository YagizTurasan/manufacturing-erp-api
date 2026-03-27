using BahadirMakina.Domain.Enums;

namespace BahadirMakina.Application.DTOs.Depo;

public class DepoDto
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Kod { get; set; } = string.Empty;
    public DepoTip Tip { get; set; }
    public string? Aciklama { get; set; }
}