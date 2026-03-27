namespace BahadirMakina.Application.DTOs.IsAdimi;

public class ParcaTamamlaDto
{
    public int IsAdimiId { get; set; }
    public int KullaniciId { get; set; }
    public decimal Miktar { get; set; } // Kaç adet tamamlandý (genelde 1)
}
