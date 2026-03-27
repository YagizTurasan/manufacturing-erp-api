namespace BahadirMakina.Application.DTOs.Istasyon;

public class IstatistikDto
{
    public int BugunkuIsAdedi { get; set; }
    public int TamamlananIsAdedi { get; set; }
    public int BekleyenIsAdedi { get; set; }
    public decimal ToplamCalismaZamani { get; set; } // Saat cinsinden
    public decimal OrtalamaCevrimeZamani { get; set; } // Dakika cinsinden
    public int BuHaftaIsSayisi { get; set; }
    public int BuAyIsSayisi { get; set; }
}