namespace BahadirMakina.Application.DTOs.Kullanici;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public KullaniciDto Kullanici { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}