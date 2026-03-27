using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BahadirMakina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initmigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Depolar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Kod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Depolar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AdSoyad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Rol = table.Column<int>(type: "int", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Urunler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Kod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    Birim = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MinimumStok = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urunler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IsEmirleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsEmriNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    HedefMiktar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TamamlananMiktar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HurdaMiktar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OlusturanKullaniciId = table.Column<int>(type: "int", nullable: false),
                    Notlar = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IsEmirleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IsEmirleri_Kullanicilar_OlusturanKullaniciId",
                        column: x => x.OlusturanKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IsEmirleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UrunAgaciAdimlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    Sira = table.Column<int>(type: "int", nullable: false),
                    IstasyonTipi = table.Column<int>(type: "int", nullable: false),
                    Tanim = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CiktiUrunId = table.Column<int>(type: "int", nullable: true),
                    KaliteKontrolGerekli = table.Column<bool>(type: "bit", nullable: false),
                    HedefDepoId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunAgaciAdimlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrunAgaciAdimlari_Depolar_HedefDepoId",
                        column: x => x.HedefDepoId,
                        principalTable: "Depolar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UrunAgaciAdimlari_Urunler_CiktiUrunId",
                        column: x => x.CiktiUrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UrunAgaciAdimlari_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UrunAgaciGerekliBilesenler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    BilesenUrunId = table.Column<int>(type: "int", nullable: false),
                    Miktar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunAgaciGerekliBilesenler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrunAgaciGerekliBilesenler_Urunler_BilesenUrunId",
                        column: x => x.BilesenUrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UrunAgaciGerekliBilesenler_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DepoHareketleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsEmriId = table.Column<int>(type: "int", nullable: true),
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    KaynakDepoId = table.Column<int>(type: "int", nullable: true),
                    HedefDepoId = table.Column<int>(type: "int", nullable: true),
                    Miktar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IstasyonId = table.Column<int>(type: "int", nullable: true),
                    IsAdimiId = table.Column<int>(type: "int", nullable: true),
                    KullaniciId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepoHareketleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepoHareketleri_Depolar_HedefDepoId",
                        column: x => x.HedefDepoId,
                        principalTable: "Depolar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepoHareketleri_Depolar_KaynakDepoId",
                        column: x => x.KaynakDepoId,
                        principalTable: "Depolar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepoHareketleri_IsEmirleri_IsEmriId",
                        column: x => x.IsEmriId,
                        principalTable: "IsEmirleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepoHareketleri_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepoHareketleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Hurdalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    Miktar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Sebep = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsEmriId = table.Column<int>(type: "int", nullable: true),
                    IsAdimiId = table.Column<int>(type: "int", nullable: true),
                    HurdaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KaliteKontrolId = table.Column<int>(type: "int", nullable: true),
                    KaynakDepoId = table.Column<int>(type: "int", nullable: true),
                    TahminiMaliyetKaybi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hurdalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hurdalar_Depolar_KaynakDepoId",
                        column: x => x.KaynakDepoId,
                        principalTable: "Depolar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Hurdalar_IsEmirleri_IsEmriId",
                        column: x => x.IsEmriId,
                        principalTable: "IsEmirleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Hurdalar_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IsAdimiBilesenleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsAdimiId = table.Column<int>(type: "int", nullable: false),
                    BilesenUrunId = table.Column<int>(type: "int", nullable: false),
                    Miktar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IsAdimiBilesenleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IsAdimiBilesenleri_Urunler_BilesenUrunId",
                        column: x => x.BilesenUrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IsAdimiLoglari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsAdimiId = table.Column<int>(type: "int", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    EskiDurum = table.Column<int>(type: "int", nullable: false),
                    YeniDurum = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IsAdimiLoglari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IsAdimiLoglari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IsAdimlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsEmriId = table.Column<int>(type: "int", nullable: false),
                    Sira = table.Column<int>(type: "int", nullable: false),
                    IstasyonId = table.Column<int>(type: "int", nullable: false),
                    Tanim = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SorumluKullaniciId = table.Column<int>(type: "int", nullable: true),
                    OncekiAdimId = table.Column<int>(type: "int", nullable: true),
                    GirdiDepoId = table.Column<int>(type: "int", nullable: true),
                    CiktiDepoId = table.Column<int>(type: "int", nullable: false),
                    GirdiUrunId = table.Column<int>(type: "int", nullable: true),
                    CiktiUrunId = table.Column<int>(type: "int", nullable: true),
                    HedefMiktar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TamamlananMiktar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KaliteKontrolGerekli = table.Column<bool>(type: "bit", nullable: false),
                    KaliteKontrolTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IsAdimlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IsAdimlari_Depolar_CiktiDepoId",
                        column: x => x.CiktiDepoId,
                        principalTable: "Depolar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IsAdimlari_Depolar_GirdiDepoId",
                        column: x => x.GirdiDepoId,
                        principalTable: "Depolar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IsAdimlari_IsAdimlari_OncekiAdimId",
                        column: x => x.OncekiAdimId,
                        principalTable: "IsAdimlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IsAdimlari_IsEmirleri_IsEmriId",
                        column: x => x.IsEmriId,
                        principalTable: "IsEmirleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IsAdimlari_Kullanicilar_SorumluKullaniciId",
                        column: x => x.SorumluKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IsAdimlari_Urunler_CiktiUrunId",
                        column: x => x.CiktiUrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IsAdimlari_Urunler_GirdiUrunId",
                        column: x => x.GirdiUrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Istasyonlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Kod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    DepoId = table.Column<int>(type: "int", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    AktifIsAdimiId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Istasyonlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Istasyonlar_Depolar_DepoId",
                        column: x => x.DepoId,
                        principalTable: "Depolar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Istasyonlar_IsAdimlari_AktifIsAdimiId",
                        column: x => x.AktifIsAdimiId,
                        principalTable: "IsAdimlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "KaliteKontroller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsAdimiId = table.Column<int>(type: "int", nullable: false),
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    DepoId = table.Column<int>(type: "int", nullable: false),
                    KontrolEdilenMiktar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OnaylananMiktar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RedMiktar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Sonuc = table.Column<int>(type: "int", nullable: false),
                    RedSebebi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    KontrolEdenKullaniciId = table.Column<int>(type: "int", nullable: false),
                    KontrolTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notlar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TekrarIslenecekMiktar = table.Column<int>(type: "int", nullable: false),
                    TekrarIslemBaslatildi = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KaliteKontroller", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KaliteKontroller_Depolar_DepoId",
                        column: x => x.DepoId,
                        principalTable: "Depolar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KaliteKontroller_IsAdimlari_IsAdimiId",
                        column: x => x.IsAdimiId,
                        principalTable: "IsAdimlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KaliteKontroller_Kullanicilar_KontrolEdenKullaniciId",
                        column: x => x.KontrolEdenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KaliteKontroller_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stoklar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepoId = table.Column<int>(type: "int", nullable: false),
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    Miktar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    IsEmriId = table.Column<int>(type: "int", nullable: true),
                    IsAdimiId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stoklar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stoklar_Depolar_DepoId",
                        column: x => x.DepoId,
                        principalTable: "Depolar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stoklar_IsAdimlari_IsAdimiId",
                        column: x => x.IsAdimiId,
                        principalTable: "IsAdimlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stoklar_IsEmirleri_IsEmriId",
                        column: x => x.IsEmriId,
                        principalTable: "IsEmirleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stoklar_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepoHareketleri_HedefDepoId",
                table: "DepoHareketleri",
                column: "HedefDepoId");

            migrationBuilder.CreateIndex(
                name: "IX_DepoHareketleri_IsAdimiId",
                table: "DepoHareketleri",
                column: "IsAdimiId");

            migrationBuilder.CreateIndex(
                name: "IX_DepoHareketleri_IsEmriId",
                table: "DepoHareketleri",
                column: "IsEmriId");

            migrationBuilder.CreateIndex(
                name: "IX_DepoHareketleri_IstasyonId",
                table: "DepoHareketleri",
                column: "IstasyonId");

            migrationBuilder.CreateIndex(
                name: "IX_DepoHareketleri_KaynakDepoId",
                table: "DepoHareketleri",
                column: "KaynakDepoId");

            migrationBuilder.CreateIndex(
                name: "IX_DepoHareketleri_KullaniciId",
                table: "DepoHareketleri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_DepoHareketleri_UrunId",
                table: "DepoHareketleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_Depolar_Kod",
                table: "Depolar",
                column: "Kod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hurdalar_IsAdimiId",
                table: "Hurdalar",
                column: "IsAdimiId");

            migrationBuilder.CreateIndex(
                name: "IX_Hurdalar_IsEmriId",
                table: "Hurdalar",
                column: "IsEmriId");

            migrationBuilder.CreateIndex(
                name: "IX_Hurdalar_KaliteKontrolId",
                table: "Hurdalar",
                column: "KaliteKontrolId");

            migrationBuilder.CreateIndex(
                name: "IX_Hurdalar_KaynakDepoId",
                table: "Hurdalar",
                column: "KaynakDepoId");

            migrationBuilder.CreateIndex(
                name: "IX_Hurdalar_UrunId",
                table: "Hurdalar",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_IsAdimiBilesenleri_BilesenUrunId",
                table: "IsAdimiBilesenleri",
                column: "BilesenUrunId");

            migrationBuilder.CreateIndex(
                name: "IX_IsAdimiBilesenleri_IsAdimiId_BilesenUrunId",
                table: "IsAdimiBilesenleri",
                columns: new[] { "IsAdimiId", "BilesenUrunId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IsAdimiLoglari_IsAdimiId",
                table: "IsAdimiLoglari",
                column: "IsAdimiId");

            migrationBuilder.CreateIndex(
                name: "IX_IsAdimiLoglari_KullaniciId",
                table: "IsAdimiLoglari",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_IsAdimlari_CiktiDepoId",
                table: "IsAdimlari",
                column: "CiktiDepoId");

            migrationBuilder.CreateIndex(
                name: "IX_IsAdimlari_CiktiUrunId",
                table: "IsAdimlari",
                column: "CiktiUrunId");

            migrationBuilder.CreateIndex(
                name: "IX_IsAdimlari_GirdiDepoId",
                table: "IsAdimlari",
                column: "GirdiDepoId");

            migrationBuilder.CreateIndex(
                name: "IX_IsAdimlari_GirdiUrunId",
                table: "IsAdimlari",
                column: "GirdiUrunId");

            migrationBuilder.CreateIndex(
                name: "IX_IsAdimlari_IsEmriId",
                table: "IsAdimlari",
                column: "IsEmriId");

            migrationBuilder.CreateIndex(
                name: "IX_IsAdimlari_IstasyonId",
                table: "IsAdimlari",
                column: "IstasyonId");

            migrationBuilder.CreateIndex(
                name: "IX_IsAdimlari_OncekiAdimId",
                table: "IsAdimlari",
                column: "OncekiAdimId");

            migrationBuilder.CreateIndex(
                name: "IX_IsAdimlari_SorumluKullaniciId",
                table: "IsAdimlari",
                column: "SorumluKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_IsEmriNo",
                table: "IsEmirleri",
                column: "IsEmriNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_OlusturanKullaniciId",
                table: "IsEmirleri",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_UrunId",
                table: "IsEmirleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_Istasyonlar_AktifIsAdimiId",
                table: "Istasyonlar",
                column: "AktifIsAdimiId");

            migrationBuilder.CreateIndex(
                name: "IX_Istasyonlar_DepoId",
                table: "Istasyonlar",
                column: "DepoId");

            migrationBuilder.CreateIndex(
                name: "IX_Istasyonlar_Kod",
                table: "Istasyonlar",
                column: "Kod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KaliteKontroller_DepoId",
                table: "KaliteKontroller",
                column: "DepoId");

            migrationBuilder.CreateIndex(
                name: "IX_KaliteKontroller_IsAdimiId",
                table: "KaliteKontroller",
                column: "IsAdimiId");

            migrationBuilder.CreateIndex(
                name: "IX_KaliteKontroller_KontrolEdenKullaniciId",
                table: "KaliteKontroller",
                column: "KontrolEdenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_KaliteKontroller_UrunId",
                table: "KaliteKontroller",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_KullaniciAdi",
                table: "Kullanicilar",
                column: "KullaniciAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stoklar_DepoId_UrunId",
                table: "Stoklar",
                columns: new[] { "DepoId", "UrunId" });

            migrationBuilder.CreateIndex(
                name: "IX_Stoklar_IsAdimiId",
                table: "Stoklar",
                column: "IsAdimiId");

            migrationBuilder.CreateIndex(
                name: "IX_Stoklar_IsEmriId",
                table: "Stoklar",
                column: "IsEmriId");

            migrationBuilder.CreateIndex(
                name: "IX_Stoklar_UrunId",
                table: "Stoklar",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunAgaciAdimlari_CiktiUrunId",
                table: "UrunAgaciAdimlari",
                column: "CiktiUrunId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunAgaciAdimlari_HedefDepoId",
                table: "UrunAgaciAdimlari",
                column: "HedefDepoId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunAgaciAdimlari_UrunId_Sira",
                table: "UrunAgaciAdimlari",
                columns: new[] { "UrunId", "Sira" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UrunAgaciGerekliBilesenler_BilesenUrunId",
                table: "UrunAgaciGerekliBilesenler",
                column: "BilesenUrunId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunAgaciGerekliBilesenler_UrunId_BilesenUrunId",
                table: "UrunAgaciGerekliBilesenler",
                columns: new[] { "UrunId", "BilesenUrunId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_Kod",
                table: "Urunler",
                column: "Kod",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DepoHareketleri_IsAdimlari_IsAdimiId",
                table: "DepoHareketleri",
                column: "IsAdimiId",
                principalTable: "IsAdimlari",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DepoHareketleri_Istasyonlar_IstasyonId",
                table: "DepoHareketleri",
                column: "IstasyonId",
                principalTable: "Istasyonlar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hurdalar_IsAdimlari_IsAdimiId",
                table: "Hurdalar",
                column: "IsAdimiId",
                principalTable: "IsAdimlari",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hurdalar_KaliteKontroller_KaliteKontrolId",
                table: "Hurdalar",
                column: "KaliteKontrolId",
                principalTable: "KaliteKontroller",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IsAdimiBilesenleri_IsAdimlari_IsAdimiId",
                table: "IsAdimiBilesenleri",
                column: "IsAdimiId",
                principalTable: "IsAdimlari",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IsAdimiLoglari_IsAdimlari_IsAdimiId",
                table: "IsAdimiLoglari",
                column: "IsAdimiId",
                principalTable: "IsAdimlari",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IsAdimlari_Istasyonlar_IstasyonId",
                table: "IsAdimlari",
                column: "IstasyonId",
                principalTable: "Istasyonlar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IsAdimlari_Depolar_CiktiDepoId",
                table: "IsAdimlari");

            migrationBuilder.DropForeignKey(
                name: "FK_IsAdimlari_Depolar_GirdiDepoId",
                table: "IsAdimlari");

            migrationBuilder.DropForeignKey(
                name: "FK_Istasyonlar_Depolar_DepoId",
                table: "Istasyonlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Istasyonlar_IsAdimlari_AktifIsAdimiId",
                table: "Istasyonlar");

            migrationBuilder.DropTable(
                name: "DepoHareketleri");

            migrationBuilder.DropTable(
                name: "Hurdalar");

            migrationBuilder.DropTable(
                name: "IsAdimiBilesenleri");

            migrationBuilder.DropTable(
                name: "IsAdimiLoglari");

            migrationBuilder.DropTable(
                name: "Stoklar");

            migrationBuilder.DropTable(
                name: "UrunAgaciAdimlari");

            migrationBuilder.DropTable(
                name: "UrunAgaciGerekliBilesenler");

            migrationBuilder.DropTable(
                name: "KaliteKontroller");

            migrationBuilder.DropTable(
                name: "Depolar");

            migrationBuilder.DropTable(
                name: "IsAdimlari");

            migrationBuilder.DropTable(
                name: "IsEmirleri");

            migrationBuilder.DropTable(
                name: "Istasyonlar");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Urunler");
        }
    }
}
