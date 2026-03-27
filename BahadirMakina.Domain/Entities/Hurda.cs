namespace BahadirMakina.Domain.Entities;

public class Hurda : BaseEntity
{
    public int UrunId { get; set; }
    public decimal Miktar { get; set; }
    public string Sebep { get; set; } = string.Empty; // "Kalite kontrol geçemedi"
    public int? IsEmriId { get; set; }
    public int? IsAdimiId { get; set; }
    public DateTime HurdaTarihi { get; set; } = DateTime.UtcNow;
    public int? KaliteKontrolId { get; set; } // Hangi kalite kontrolden reddedildi
    public int? KaynakDepoId { get; set; } // Hangi depodan hurdaya ayrýldý
    public decimal? TahminiMaliyetKaybi { get; set; } // Hurda maliyeti
    public Urun Urun { get; set; } = null!;
    public IsEmri? IsEmri { get; set; }
    public IsAdimi? IsAdimi { get; set; }
    public KaliteKontrol? KaliteKontrol { get; set; }
    public Depo? KaynakDepo { get; set; }
}