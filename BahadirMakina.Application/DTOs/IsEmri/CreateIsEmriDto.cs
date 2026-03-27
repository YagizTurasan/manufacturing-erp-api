namespace BahadirMakina.Application.DTOs.IsEmri;

public class CreateIsEmriDto
{
    public int UrunId { get; set; }
    public decimal HedefMiktar { get; set; }
    public string? Notlar { get; set; }
}