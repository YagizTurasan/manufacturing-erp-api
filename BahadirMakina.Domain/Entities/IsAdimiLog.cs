using BahadirMakina.Domain.Enums;

namespace BahadirMakina.Domain.Entities;

public class IsAdimiLog : BaseEntity
{
    public int IsAdimiId { get; set; }
    public int KullaniciId { get; set; }
    public IsAdimiDurum EskiDurum { get; set; }
    public IsAdimiDurum YeniDurum { get; set; }
    public string? Aciklama { get; set; }
    public DateTime IslemTarihi { get; set; } = DateTime.UtcNow;
    public IsAdimi IsAdimi { get; set; } = null!;
    public Kullanici Kullanici { get; set; } = null!;
}