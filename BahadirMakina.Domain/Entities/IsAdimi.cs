using BahadirMakina.Domain.Enums;

namespace BahadirMakina.Domain.Entities;

public class IsAdimi : BaseEntity
{
    public int IsEmriId { get; set; }
    public int Sira { get; set; }
    public int IstasyonId { get; set; }
    public string Tanim { get; set; } = string.Empty;
    public IsAdimiDurum Durum { get; set; } = IsAdimiDurum.Beklemede;
    public DateTime? BaslangicTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
    public int? SorumluKullaniciId { get; set; }
    public int? OncekiAdimId { get; set; }

    public int? GirdiDepoId { get; set; }
    public int CiktiDepoId { get; set; }
    public int? GirdiUrunId { get; set; }
    public int? CiktiUrunId { get; set; }
    public decimal HedefMiktar { get; set; }
    public decimal TamamlananMiktar { get; set; } = 0;
    public bool KaliteKontrolGerekli { get; set; } = true;
    public DateTime? KaliteKontrolTarihi { get; set; }
    public IsEmri IsEmri { get; set; } = null!;
    public Istasyon Istasyon { get; set; } = null!;
    public Kullanici? SorumluKullanici { get; set; }
    public IsAdimi? OncekiAdim { get; set; }
    public ICollection<IsAdimi> SonrakiAdimlar { get; set; } = new List<IsAdimi>();
    public ICollection<IsAdimiLog> Loglar { get; set; } = new List<IsAdimiLog>();
    public Depo? GirdiDepo { get; set; }
    public Depo CiktiDepo { get; set; } = null!;
    public Urun? GirdiUrun { get; set; }
    public Urun? CiktiUrun { get; set; }
    public ICollection<KaliteKontrol> KaliteKontroller { get; set; } = new List<KaliteKontrol>();
    public ICollection<DepoHareket> DepoHareketleri { get; set; } = new List<DepoHareket>();
    public ICollection<IsAdimiBilesen> GerekliBilesenler { get; set; } = new List<IsAdimiBilesen>();
}