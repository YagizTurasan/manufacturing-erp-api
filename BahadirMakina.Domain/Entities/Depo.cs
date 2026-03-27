using BahadirMakina.Domain.Enums;

namespace BahadirMakina.Domain.Entities;

public class Depo : BaseEntity
{
    public string Ad { get; set; } = string.Empty; // "Hammadde Deposu", "Talaþlý Ýmalat Deposu"
    public string Kod { get; set; } = string.Empty; // "HAMMADDE", "TALASLI_IMALAT"
    public DepoTip Tip { get; set; }
    public string? Aciklama { get; set; }
    public ICollection<Stok> Stoklar { get; set; } = new List<Stok>();
    public ICollection<DepoHareket> DepoHareketleri { get; set; } = new List<DepoHareket>();
}