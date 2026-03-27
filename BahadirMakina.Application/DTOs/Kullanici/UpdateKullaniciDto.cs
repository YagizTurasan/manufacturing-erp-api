using System.ComponentModel.DataAnnotations;

namespace BahadirMakina.Application.DTOs.Kullanici;

public class UpdateKullaniciDto
{
    public int Id { get; set; }

    [StringLength(100)]
    public string? AdSoyad { get; set; }

    public bool? Aktif { get; set; }
}