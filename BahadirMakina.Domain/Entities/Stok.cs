using BahadirMakina.Domain.Enums;

namespace BahadirMakina.Domain.Entities;

public class Stok : BaseEntity
{
    public int DepoId { get; set; }
    public int UrunId { get; set; }
    public decimal Miktar { get; set; }
    public StokDurum Durum { get; set; } // Hazir, IslemBekliyor, IslemdeKullaniliyor
    public int? IsEmriId { get; set; } // Bu stok hangi iþ emrine baðlý
    public int? IsAdimiId { get; set; } // Bu stok hangi iþ adýmýnda kullanýlýyor
    public Depo Depo { get; set; } = null!;
    public Urun Urun { get; set; } = null!;
    public IsEmri? IsEmri { get; set; }
    public IsAdimi? IsAdimi { get; set; }
}