using BahadirMakina.Domain.Enums;

namespace BahadirMakina.Domain.Entities;

public class Urun : BaseEntity
{
    public string Ad { get; set; } = string.Empty;
    public string Kod { get; set; } = string.Empty;
    public UrunTip Tip { get; set; }
    public string Birim { get; set; } = "Adet";
    public string? Aciklama { get; set; }
    public decimal? MinimumStok { get; set; }
    public ICollection<Stok> Stoklar { get; set; } = new List<Stok>();
    public ICollection<UrunAgaciAdim> UrunAgaciAdimlari { get; set; } = new List<UrunAgaciAdim>();
    public ICollection<UrunAgaciGerekliBilesen> GerekliOlduguUrunler { get; set; } = new List<UrunAgaciGerekliBilesen>();
    public ICollection<UrunAgaciGerekliBilesen> BilesenOlduguUrunler { get; set; } = new List<UrunAgaciGerekliBilesen>();
}