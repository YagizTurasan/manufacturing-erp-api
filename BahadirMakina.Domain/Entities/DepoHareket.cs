using BahadirMakina.Domain.Enums;

namespace BahadirMakina.Domain.Entities;

public class DepoHareket : BaseEntity
{
    public int? IsEmriId { get; set; }
    public int UrunId { get; set; }
    public int? KaynakDepoId { get; set; } // Nereden
    public int? HedefDepoId { get; set; } // Nereye
    public decimal Miktar { get; set; }
    public DepoHareketTip Tip { get; set; } // Giris, Cikis, Transfer, Hurda
    public string? Aciklama { get; set; }
    public DateTime IslemTarihi { get; set; } = DateTime.UtcNow;
    public int? IstasyonId { get; set; } // Hangi istasyonda iþlem yapýldý
    public int? IsAdimiId { get; set; } // Hangi iþ adýmýnda bu hareket oldu
    public int? KullaniciId { get; set; } // Kim bu hareketi yaptý
    public IsEmri? IsEmri { get; set; }
    public Urun Urun { get; set; } = null!;
    public Depo? KaynakDepo { get; set; }
    public Depo? HedefDepo { get; set; }
    public Istasyon? Istasyon { get; set; }
    public IsAdimi? IsAdimi { get; set; }
    public Kullanici? Kullanici { get; set; }
}