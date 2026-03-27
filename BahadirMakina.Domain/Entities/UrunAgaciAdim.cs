using BahadirMakina.Domain.Enums;

namespace BahadirMakina.Domain.Entities;

public class UrunAgaciAdim : BaseEntity
{
    public int UrunId { get; set; }
    public int Sira { get; set; }
    public IstasyonTip IstasyonTipi { get; set; }
    public string Tanim { get; set; } = string.Empty;

    public int? CiktiUrunId { get; set; }
    public bool KaliteKontrolGerekli { get; set; } = true;
    public int HedefDepoId { get; set; }
    public Urun Urun { get; set; } = null!;
    public Urun? CiktiUrun { get; set; }
    public Depo HedefDepo { get; set; } = null!;
}