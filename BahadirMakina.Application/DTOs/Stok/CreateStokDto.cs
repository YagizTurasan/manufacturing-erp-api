namespace BahadirMakina.Application.DTOs.Stok;

public class CreateStokDto
{
    public int DepoId { get; set; }
    public int UrunId { get; set; }
    public decimal Miktar { get; set; }
}