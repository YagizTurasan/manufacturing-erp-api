namespace BahadirMakina.Domain.Enums;

public enum StokDurum
{
    Hazir = 1,          // Kullanýma hazýr
    IslemBekliyor = 2,  // Ýþ emrinde ama henüz iþlenmedi
    Rezerve = 3,
    Hurda = 4
}