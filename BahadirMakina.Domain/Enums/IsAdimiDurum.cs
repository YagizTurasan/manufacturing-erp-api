namespace BahadirMakina.Domain.Enums;

public enum IsAdimiDurum
{
    Beklemede = 1,        // Önceki adým tamamlanmadý veya malzeme hazýr deðil
    Hazir = 2,            // Baþlatýlabilir (önceki adým tamam, malzeme hazýr)
    DevamEdiyor = 3,      // Operatör iþlemi yapýyor
    KaliteKontrolde = 4,  // Ýþlem bitti, kalite kontrol yapýlýyor
    Tamamlandi = 5,       // Baþarýyla tamamlandý
    Reddedildi = 6,
}