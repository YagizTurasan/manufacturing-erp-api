using BahadirMakina.Domain.Entities;
using BahadirMakina.Domain.Enums;

namespace BahadirMakina.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (context.Kullanicilar.Any()) return;

        Console.WriteLine("Seed data baþlýyor...");
        Console.WriteLine("Kullanýcýlar oluþturuluyor...");

        var admin = new Kullanici
        {
            KullaniciAdi = "admin",
            AdSoyad = "Admin Kullanýcý",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Rol = KullaniciRol.Admin,
            Aktif = true,
            CreatedAt = DateTime.UtcNow
        };

        var dogrultmaci1 = new Kullanici
        {
            KullaniciAdi = "dogrultmaci1",
            AdSoyad = "Hasan Yýlmaz",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
            Rol = KullaniciRol.DogrultmaOperatoru,
            Aktif = true,
            CreatedAt = DateTime.UtcNow
        };

        var dikIslemci1 = new Kullanici
        {
            KullaniciAdi = "dikislemci1",
            AdSoyad = "Recep Yýlmaz",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
            Rol = KullaniciRol.DikIslemci,
            Aktif = true,
            CreatedAt = DateTime.UtcNow
        };

        var presci1 = new Kullanici
        {
            KullaniciAdi = "presci1",
            AdSoyad = "Hasan Yýldýrým",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
            Rol = KullaniciRol.Presci,
            Aktif = true,
            CreatedAt = DateTime.UtcNow
        };

        var lazerci1 = new Kullanici
        {
            KullaniciAdi = "lazerci1",
            AdSoyad = "Ahmet Yýldýrým",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
            Rol = KullaniciRol.Lazerci,
            Aktif = true,
            CreatedAt = DateTime.UtcNow
        };

        var tornaci1 = new Kullanici
        {
            KullaniciAdi = "tornaci1",
            AdSoyad = "Ahmet Yýlmaz",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
            Rol = KullaniciRol.Tornaci,
            Aktif = true,
            CreatedAt = DateTime.UtcNow
        };

        var tornaci2 = new Kullanici
        {
            KullaniciAdi = "tornaci2",
            AdSoyad = "Mehmet Çelik",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
            Rol = KullaniciRol.Tornaci,
            Aktif = true,
            CreatedAt = DateTime.UtcNow
        };

        var kaynakci1 = new Kullanici
        {
            KullaniciAdi = "kaynakci1",
            AdSoyad = "Mehmet Demir",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
            Rol = KullaniciRol.Kaynakci,
            Aktif = true,
            CreatedAt = DateTime.UtcNow
        };

        var kaliteKontrol = new Kullanici
        {
            KullaniciAdi = "kalite1",
            AdSoyad = "Fatma Kaya",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
            Rol = KullaniciRol.KaliteKontrol,
            Aktif = true,
            CreatedAt = DateTime.UtcNow
        };

        var abkantci1 = new Kullanici
        {
            KullaniciAdi = "abkantci1",
            AdSoyad = "Ali Þahin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
            Rol = KullaniciRol.Abkantci,
            Aktif = true,
            CreatedAt = DateTime.UtcNow
        };

        var kumlamaci1 = new Kullanici
        {
            KullaniciAdi = "kumlamaci1",
            AdSoyad = "Yaðýz Turasan",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
            Rol = KullaniciRol.Kumlamaci,
            Aktif = true,
            CreatedAt = DateTime.UtcNow
        };

        var taslamaci1 = new Kullanici
        {   
            KullaniciAdi = "taslamaci1",
            AdSoyad = "Bayram Yaðcý",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
            Rol = KullaniciRol.Taslamaci,
            Aktif = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Kullanicilar.AddRange(admin, tornaci1, tornaci2, kaynakci1, kaliteKontrol, abkantci1, kumlamaci1, taslamaci1, dogrultmaci1, dikIslemci1, presci1, lazerci1);
        await context.SaveChangesAsync();
        Console.WriteLine("? Kullanýcýlar oluþturuldu");
        Console.WriteLine("Depolar oluþturuluyor...");

        var hammaddeDepo = new Depo
        {
            Ad = "Hammadde Deposu",
            Kod = "HAMMADDE",
            Tip = DepoTip.Hammadde,
            Aciklama = "Ham malzemeler",
            CreatedAt = DateTime.UtcNow
        };

        var yariMamulDepo = new Depo
        {
            Ad = "Yarý Mamül Deposu",
            Kod = "YARI_MAMUL",
            Tip = DepoTip.YariMamul,
            Aciklama = "Ýþlenmiþ ara ürünler",
            CreatedAt = DateTime.UtcNow
        };

        var sevkDepo = new Depo
        {
            Ad = "Sevk Deposu",
            Kod = "SEVK",
            Tip = DepoTip.Sevk,
            Aciklama = "Sevkiyata hazýr mamüller",
            CreatedAt = DateTime.UtcNow
        };

        var hurdaDepo = new Depo
        {
            Ad = "Hurda Deposu",
            Kod = "HURDA",
            Tip = DepoTip.Hurda,
            Aciklama = "Kalite kontrolden geçemeyen ürünler",
            CreatedAt = DateTime.UtcNow
        };

        var talasliImalatDepo = new Depo
        {
            Ad = "Talaþlý Ýmalat Deposu",
            Kod = "TALASLI_IMALAT",
            Tip = DepoTip.TalasliImalat,
            Aciklama = "Torna ve Dik Ýþlem istasyonlarý deposu",
            CreatedAt = DateTime.UtcNow
        };

        var kaynakDepo = new Depo
        {
            Ad = "Kaynak Deposu",
            Kod = "KAYNAK",
            Tip = DepoTip.Kaynak,
            Aciklama = "Kaynak istasyonlarý deposu",
            CreatedAt = DateTime.UtcNow
        };

        var presDepo = new Depo
        {
            Ad = "Pres Deposu",
            Kod = "PRES",
            Tip = DepoTip.Pres,
            Aciklama = "Hidrolik Pres istasyonlarý deposu",
            CreatedAt = DateTime.UtcNow
        };

        var abkantDepo = new Depo
        {
            Ad = "Abkant Deposu",
            Kod = "ABKANT",
            Tip = DepoTip.Abkant,
            Aciklama = "Abkant istasyonlarý deposu",
            CreatedAt = DateTime.UtcNow
        };

        var lazerDepo = new Depo
        {
            Ad = "Lazer Deposu",
            Kod = "LAZER",
            Tip = DepoTip.Lazer,
            Aciklama = "Lazer kesim istasyonlarý deposu",
            CreatedAt = DateTime.UtcNow
        };

        var taslamaDepo = new Depo
        {
            Ad = "Taþlama Deposu",
            Kod = "TASLAMA",
            Tip = DepoTip.Taslama,
            Aciklama = "Taþlama istasyonlarý deposu",
            CreatedAt = DateTime.UtcNow
        };

        var kumlamaDepo = new Depo
        {
            Ad = "Kumlama Deposu",
            Kod = "KUMLAMA",
            Tip = DepoTip.Kumlama,
            Aciklama = "Kumlama istasyonlarý deposu",
            CreatedAt = DateTime.UtcNow
        };

        var dogrultmaDepo = new Depo
        {
            Ad = "Doðrultma Deposu",
            Kod = "DOGRULTMA",
            Tip = DepoTip.Dogrultma,
            Aciklama = "Doðrultma istasyonlarý deposu",
            CreatedAt = DateTime.UtcNow
        };

        context.Depolar.AddRange(
            hammaddeDepo, yariMamulDepo, sevkDepo, hurdaDepo,
            talasliImalatDepo, kaynakDepo, presDepo, abkantDepo,
            lazerDepo, taslamaDepo, kumlamaDepo, dogrultmaDepo
        );
        await context.SaveChangesAsync();
        Console.WriteLine("? Depolar oluþturuldu");
        Console.WriteLine("Ýstasyonlar oluþturuluyor...");

        var torna1 = new Istasyon
        {
            Ad = "Torna 1",
            Kod = "TRN-01",
            Tip = IstasyonTip.Torna,
            DepoId = talasliImalatDepo.Id,
            Durum = IstasyonDurum.Musait,
            CreatedAt = DateTime.UtcNow
        };

        var torna2 = new Istasyon
        {
            Ad = "Torna 2",
            Kod = "TRN-02",
            Tip = IstasyonTip.Torna,
            DepoId = talasliImalatDepo.Id,
            Durum = IstasyonDurum.Musait,
            CreatedAt = DateTime.UtcNow
        };

        var gazaltiKaynak1 = new Istasyon
        {
            Ad = "Gazaltý Kaynak 1",
            Kod = "GAZ-01",
            Tip = IstasyonTip.GazaltiKaynak,
            DepoId = kaynakDepo.Id,
            Durum = IstasyonDurum.Musait,
            CreatedAt = DateTime.UtcNow
        };

        var robotKaynak1 = new Istasyon
        {
            Ad = "Robot Kaynak 1",
            Kod = "RBT-01",
            Tip = IstasyonTip.RobotKaynak,
            DepoId = kaynakDepo.Id,
            Durum = IstasyonDurum.Musait,
            CreatedAt = DateTime.UtcNow
        };

        var kumlama1 = new Istasyon
        {
            Ad = "Kumlama 1",
            Kod = "KML-01",
            Tip = IstasyonTip.Kumlama,
            DepoId = kumlamaDepo.Id,
            Durum = IstasyonDurum.Musait,
            CreatedAt = DateTime.UtcNow
        };

        var taslama1 = new Istasyon
        {
            Ad = "Taþlama 1",
            Kod = "TSL-01",
            Tip = IstasyonTip.Taslama,
            DepoId = taslamaDepo.Id,
            Durum = IstasyonDurum.Musait,
            CreatedAt = DateTime.UtcNow
        };

        var abkant1 = new Istasyon
        {
            Ad = "Abkant 1",
            Kod = "ABK-01",
            Tip = IstasyonTip.Abkant,
            DepoId = abkantDepo.Id,
            Durum = IstasyonDurum.Musait,
            CreatedAt = DateTime.UtcNow
        };

        var lazer1 = new Istasyon
        {
            Ad = "Lazer 1",
            Kod = "LZR-01",
            Tip = IstasyonTip.Lazer,
            DepoId = lazerDepo.Id,
            Durum = IstasyonDurum.Musait,
            CreatedAt = DateTime.UtcNow
        };

        var press1 = new Istasyon
        {
            Ad = "Press 1",
            Kod = "PRS-01",
            Tip = IstasyonTip.HidrolikPres,
            DepoId = presDepo.Id,
            Durum = IstasyonDurum.Musait,
            CreatedAt = DateTime.UtcNow
        };

        var dogrultma1 = new Istasyon
        {
            Ad = "Dogrultma 1",
            Kod = "DGR-01",
            Tip = IstasyonTip.Dogrultma,
            DepoId = dogrultmaDepo.Id,
            Durum = IstasyonDurum.Musait,
            CreatedAt = DateTime.UtcNow
        };

        context.Istasyonlar.AddRange(
            torna1, torna2, gazaltiKaynak1, robotKaynak1,
            kumlama1, taslama1, abkant1, lazer1, press1, dogrultma1
        );
        await context.SaveChangesAsync();
        Console.WriteLine("? Ýstasyonlar oluþturuldu");
        Console.WriteLine("Ürünler oluþturuluyor...");
        var mil12 = new Urun
        {
            Ad = "Ø12 - Mil 1040",
            Kod = "MIL-12-1040",
            Tip = UrunTip.Hammadde,
            Birim = "Adet",
            MinimumStok = 100,
            CreatedAt = DateTime.UtcNow
        };

        var sac8mm = new Urun
        {
            Ad = "8mm 3237 Siyah Sac",
            Kod = "SAC-8-3237",
            Tip = UrunTip.Hammadde,
            Birim = "Adet",
            MinimumStok = 100,
            CreatedAt = DateTime.UtcNow
        };

        var sac5mm = new Urun
        {
            Ad = "5mm 3237 Siyah Sac",
            Kod = "SAC-5-3237",
            Tip = UrunTip.Hammadde,
            Birim = "Adet",
            MinimumStok = 100,
            CreatedAt = DateTime.UtcNow
        };
        var jamper1 = new Urun
        {
            Ad = "Jamper-1",
            Kod = "JAMPER-1",
            Tip = UrunTip.YariMamul,
            Birim = "Adet",
            MinimumStok = 10,
            CreatedAt = DateTime.UtcNow
        };

        var tabanSaci = new Urun
        {
            Ad = "Taban Sacý",
            Kod = "TABAN-SACI",
            Tip = UrunTip.YariMamul,
            Birim = "Adet",
            MinimumStok = 10,
            CreatedAt = DateTime.UtcNow
        };

        var konsol1 = new Urun
        {
            Ad = "Konsol-1",
            Kod = "KONSOL-1",
            Tip = UrunTip.YariMamul,
            Birim = "Adet",
            MinimumStok = 5,
            CreatedAt = DateTime.UtcNow
        };

        var konsol2 = new Urun
        {
            Ad = "Konsol-2",
            Kod = "KONSOL-2",
            Tip = UrunTip.YariMamul,
            Birim = "Adet",
            MinimumStok = 5,
            CreatedAt = DateTime.UtcNow
        };
        var mege10053006 = new Urun
        {
            Ad = "MEGE-10053006",
            Kod = "MEGE-10053006",
            Tip = UrunTip.Mamul,
            Birim = "Adet",
            CreatedAt = DateTime.UtcNow
        };

        context.Urunler.AddRange(
            mil12, sac8mm, sac5mm,
            jamper1, tabanSaci, konsol1, konsol2,
            mege10053006
        );
        await context.SaveChangesAsync();
        Console.WriteLine("? Ürünler oluþturuldu");
        Console.WriteLine("Ürün aðaçlarý oluþturuluyor...");
        Console.WriteLine("Stoklar oluþturuluyor...");

        context.Stoklar.AddRange(
            new Stok
            {
                DepoId = hammaddeDepo.Id,
                UrunId = mil12.Id,
                Miktar = 97, // Düþük stok (minimum: 100)
                Durum = StokDurum.Hazir,
                CreatedAt = DateTime.UtcNow
            },
            new Stok
            {
                DepoId = hammaddeDepo.Id,
                UrunId = sac8mm.Id,
                Miktar = 200,
                Durum = StokDurum.Hazir,
                CreatedAt = DateTime.UtcNow
            },
            new Stok
            {
                DepoId = hammaddeDepo.Id,
                UrunId = sac5mm.Id,
                Miktar = 150,
                Durum = StokDurum.Hazir,
                CreatedAt = DateTime.UtcNow
            },
            new Stok
            {
                DepoId = yariMamulDepo.Id,
                UrunId = jamper1.Id,
                Miktar = 13,
                Durum = StokDurum.Hazir,
                CreatedAt = DateTime.UtcNow
            },
            new Stok
            {
                DepoId = yariMamulDepo.Id,
                UrunId = tabanSaci.Id,
                Miktar = 8,
                Durum = StokDurum.Hazir,
                CreatedAt = DateTime.UtcNow
            },
            new Stok
            {
                DepoId = yariMamulDepo.Id,
                UrunId = konsol1.Id,
                Miktar = 6,
                Durum = StokDurum.Hazir,
                CreatedAt = DateTime.UtcNow
            },
            new Stok
            {
                DepoId = yariMamulDepo.Id,
                UrunId = konsol2.Id,
                Miktar = 10,
                Durum = StokDurum.Hazir,
                CreatedAt = DateTime.UtcNow
            },
            new Stok
            {
                DepoId = sevkDepo.Id,
                UrunId = mege10053006.Id,
                Miktar = 2,
                Durum = StokDurum.Hazir,
                CreatedAt = DateTime.UtcNow
            }
        );
        await context.SaveChangesAsync();
        Console.WriteLine("? Stoklar oluþturuldu");
        Console.WriteLine("Geçmiþ veriler oluþturuluyor...");

        
        Console.WriteLine("? Geçmiþ veriler oluþturuldu");
        Console.WriteLine("========================================");
        Console.WriteLine("? Seed data tamamlandý!");
        Console.WriteLine("========================================");
    }
}