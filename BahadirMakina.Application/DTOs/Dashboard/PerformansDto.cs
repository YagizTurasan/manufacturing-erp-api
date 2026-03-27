namespace BahadirMakina.Application.DTOs.Dashboard;

public class PerformansDto
{
    public DateTime Tarih { get; set; }
    public decimal Verimlilik { get; set; } // Tamamlanan / Hedef * 100
    public int UretilenAdet { get; set; }
    public int HedefAdet { get; set; }
    public int HurdaAdet { get; set; }
    public decimal HurdaOrani { get; set; }
    public decimal OrtalamaCevrimeZamani { get; set; } // Dakika
}