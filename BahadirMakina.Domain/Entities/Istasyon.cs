using BahadirMakina.Domain.Enums;

namespace BahadirMakina.Domain.Entities;

public class Istasyon : BaseEntity
{
    public string Ad { get; set; } = string.Empty; // "Torna 1"
    public string Kod { get; set; } = string.Empty; // "TRN-01"
    public IstasyonTip Tip { get; set; }
    public int DepoId { get; set; } // Hangi depoya baðlý (TalasliImalat, Kaynak, vb.)
    public IstasyonDurum Durum { get; set; } = IstasyonDurum.Musait;
    public int? AktifIsAdimiId { get; set; } // Þu an hangi iþ adýmýný yapýyor
    public Depo Depo { get; set; } = null!;
    public IsAdimi? AktifIsAdimi { get; set; }
    public ICollection<IsAdimi> IsAdimlari { get; set; } = new List<IsAdimi>();
}