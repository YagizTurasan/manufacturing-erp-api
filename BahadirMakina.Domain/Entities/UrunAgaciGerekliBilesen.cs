namespace BahadirMakina.Domain.Entities;

public class UrunAgaciGerekliBilesen : BaseEntity
{
    public int UrunId { get; set; } // Ana ürün (MEGE-10053006)
    public int BilesenUrunId { get; set; } // Gerekli bileþen (Jamper-1, Konsol-1, vb.)
    public decimal Miktar { get; set; } // Kaç adet gerekli
    public Urun Urun { get; set; } = null!;
    public Urun BilesenUrun { get; set; } = null!;
}