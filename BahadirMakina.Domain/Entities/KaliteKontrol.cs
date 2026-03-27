using BahadirMakina.Domain.Enums;

namespace BahadirMakina.Domain.Entities;

public class KaliteKontrol : BaseEntity
{
    public int IsAdimiId { get; set; }
    public int UrunId { get; set; }
    public int DepoId { get; set; } // Hangi depoda yapýldý
    public decimal KontrolEdilenMiktar { get; set; }
    public decimal OnaylananMiktar { get; set; }
    public decimal RedMiktar { get; set; }
    public KaliteKontrolSonuc Sonuc { get; set; }
    public string? RedSebebi { get; set; } // "Tolerans dýþý ölçü +0.08mm"
    public int KontrolEdenKullaniciId { get; set; }
    public DateTime KontrolTarihi { get; set; } = DateTime.UtcNow;
    public string? Notlar { get; set; }
    public int TekrarIslenecekMiktar { get; set; } // Red edilen miktar (tekrar iþlenecek)
    public bool TekrarIslemBaslatildi { get; set; } // Tekrar iþlem iþ adýmlarý oluþturuldu mu?
    public IsAdimi IsAdimi { get; set; } = null!;
    public Urun Urun { get; set; } = null!;
    public Depo Depo { get; set; } = null!;
    public Kullanici KontrolEdenKullanici { get; set; } = null!;
}