using BahadirMakina.Domain.Enums;

namespace BahadirMakina.Domain.Entities;

public class IsEmri : BaseEntity
{
    public string IsEmriNo { get; set; } = string.Empty; // "IE-2025-001"
    public int UrunId { get; set; }
    public decimal HedefMiktar { get; set; }
    public decimal TamamlananMiktar { get; set; } = 0;
    public decimal HurdaMiktar { get; set; } = 0;
    public IsEmriDurum Durum { get; set; } = IsEmriDurum.Beklemede;
    public DateTime? BaslangicTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
    public int OlusturanKullaniciId { get; set; }
    public string? Notlar { get; set; }
    public Urun Urun { get; set; } = null!;
    public Kullanici OlusturanKullanici { get; set; } = null!;
    public ICollection<IsAdimi> IsAdimlari { get; set; } = new List<IsAdimi>();
    public ICollection<DepoHareket> DepoHareketleri { get; set; } = new List<DepoHareket>();
}