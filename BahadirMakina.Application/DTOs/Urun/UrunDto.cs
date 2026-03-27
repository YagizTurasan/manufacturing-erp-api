namespace BahadirMakina.Application.DTOs.Urun;

public class UrunDto
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Kod { get; set; } = string.Empty;
    public string Tip { get; set; } = string.Empty;
    public string Birim { get; set; } = string.Empty;
    public decimal? MinimumStok { get; set; }
    public decimal ToplamStok { get; set; }
    public int AdimSayisi { get; set; }
    public DateTime CreatedAt { get; set; }
}