public class CreateKaliteKontrolDto
{
    public int IsAdimiId { get; set; }
    public decimal KontrolEdilenMiktar { get; set; }
    public decimal OnaylananMiktar { get; set; }
    public decimal RedMiktar { get; set; }
    public string? RedSebebi { get; set; }
    public int KontrolEdenKullaniciId { get; set; }
    public string? Notlar { get; set; }
    public int? GeriDonusAdimiId { get; set; } // Null ise ilk adým varsayýlan
}