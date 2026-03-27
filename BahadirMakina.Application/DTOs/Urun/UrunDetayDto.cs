namespace BahadirMakina.Application.DTOs.Urun;

public class UrunDetayDto
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Kod { get; set; } = string.Empty;
    public string Tip { get; set; } = string.Empty;
    public string Birim { get; set; } = string.Empty;
    public decimal? MinimumStok { get; set; }
    public List<StokDetayDto> Stoklar { get; set; } = new();
    public List<UrunAgaciAdimDto> UretimAdimlari { get; set; } = new();
    public List<GerekliBilesenDetayDto> GerekliBilesenler { get; set; } = new();
    public UretimIstatistikDto Istatistikler { get; set; } = new();
}

public class StokDetayDto
{
    public string DepoAdi { get; set; } = string.Empty;
    public decimal Miktar { get; set; }
    public string Durum { get; set; } = string.Empty;
}

public class UrunAgaciAdimDto
{
    public int Id { get; set; }
    public int Sira { get; set; }
    public string IstasyonTipi { get; set; } = string.Empty;
    public string Tanim { get; set; } = string.Empty;
    public string? CiktiUrunAdi { get; set; }
    public bool KaliteKontrolGerekli { get; set; }
    public string? HedefDepoAdi { get; set; }
}

public class GerekliBilesenDetayDto
{
    public int Id { get; set; }
    public string BilesenAdi { get; set; } = string.Empty;
    public string BilesenKodu { get; set; } = string.Empty;
    public decimal Miktar { get; set; }
    public decimal MevcutStok { get; set; }
}

public class UretimIstatistikDto
{
    public int ToplamUretilenAdet { get; set; }
    public int ToplamHurdaAdet { get; set; }
    public decimal HurdaOrani { get; set; }
    public int AktifIsEmriSayisi { get; set; }
}