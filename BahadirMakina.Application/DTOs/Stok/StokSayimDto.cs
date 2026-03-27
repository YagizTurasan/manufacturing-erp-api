namespace BahadirMakina.Application.DTOs.Stok;

public class StokSayimDto
{
    public int UrunId { get; set; }
    public int DepoId { get; set; }
    public decimal YeniMiktar { get; set; }
    public string? Aciklama { get; set; }
    public int KullaniciId { get; set; }
}