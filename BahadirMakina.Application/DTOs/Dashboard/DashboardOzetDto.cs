namespace BahadirMakina.Application.DTOs.Dashboard;

public class DashboardOzetDto
{
    public IsEmriOzetDto IsEmirleri { get; set; } = new();
    public IstasyonOzetDto Istasyonlar { get; set; } = new();
    public UretimOzetDto Uretim { get; set; } = new();
    public HurdaOzetDto Hurda { get; set; } = new();
    public List<AktifIsDto> AktifIsler { get; set; } = new();
    public List<DusukStokUyariDto> DusukStokUyarilari { get; set; } = new();
}

public class IsEmriOzetDto
{
    public int ToplamAktif { get; set; }
    public int Beklemede { get; set; }
    public int DevamEdiyor { get; set; }
    public int BugunkuTamamlanan { get; set; }
    public int BuHaftaTamamlanan { get; set; }
    public int BuAyTamamlanan { get; set; }
}

public class IstasyonOzetDto
{
    public int ToplamIstasyon { get; set; }
    public int Musait { get; set; }
    public int Mesgul { get; set; }
    public int Arizali { get; set; }
    public decimal KullanimOrani { get; set; } // Meþgul / Toplam * 100
}

public class UretimOzetDto
{
    public int BugunkuUretim { get; set; }
    public int BuHaftaUretim { get; set; }
    public int BuAyUretim { get; set; }
    public decimal BugunkuHedefTutturmaOrani { get; set; }
    public int BekleyenKaliteKontrol { get; set; }
}

public class HurdaOzetDto
{
    public int BugunkuHurdaAdet { get; set; }
    public int BuHaftaHurdaAdet { get; set; }
    public int BuAyHurdaAdet { get; set; }
    public decimal BugunkuHurdaOrani { get; set; }
    public decimal BuAyHurdaOrani { get; set; }
    public decimal TahminiMaliyetKaybi { get; set; }
}

public class AktifIsDto
{
    public string IsEmriNo { get; set; } = string.Empty;
    public string UrunAdi { get; set; } = string.Empty;
    public string IstasyonAdi { get; set; } = string.Empty;
    public string OperatorAdi { get; set; } = string.Empty;
    public decimal TamamlanmaYuzdesi { get; set; }
    public int KalanSure { get; set; } // Dakika
    public DateTime BaslangicTarihi { get; set; }
}

public class DusukStokUyariDto
{
    public string UrunAdi { get; set; } = string.Empty;
    public string UrunKodu { get; set; } = string.Empty;
    public decimal MevcutStok { get; set; }
    public decimal MinimumStok { get; set; }
    public decimal EksikMiktar { get; set; }
}