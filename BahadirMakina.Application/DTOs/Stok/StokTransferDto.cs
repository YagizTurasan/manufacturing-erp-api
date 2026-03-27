namespace BahadirMakina.Application.DTOs.Stok;

public class StokTransferDto
{
    public int UrunId { get; set; }
    public int KaynakDepoId { get; set; }
    public int HedefDepoId { get; set; }
    public decimal Miktar { get; set; }
    public string? Aciklama { get; set; }
    public int KullaniciId { get; set; }
}