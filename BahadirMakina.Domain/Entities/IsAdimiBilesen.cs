namespace BahadirMakina.Domain.Entities;
public class IsAdimiBilesen : BaseEntity
{
    public int IsAdimiId { get; set; }
    public int BilesenUrunId { get; set; } // Hangi bileþen gerekli
    public decimal Miktar { get; set; } // Kaç adet
    public IsAdimi IsAdimi { get; set; } = null!;
    public Urun Urun { get; set; } = null!;
}