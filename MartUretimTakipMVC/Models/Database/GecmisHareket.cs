using PagedList;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MartUretimTakipMVC.Models.Database
{
    public class GecmisHareket
    {
        [Key]
        public int HareketID { get; set; }
        public string EklenmeTarihi { get; set; }

        public string DuzenlenmeTarihi { get; set; }

        [Column(TypeName = "Nvarchar")]
        [StringLength(20)]
        [Display(Name = "Stok Kodu")]
        public string StokKodu { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(20)]
        [Display(Name = "DID")]
        public string DID { get; set; }

        [Column(TypeName = "Nvarchar")]
        [StringLength(3)]
        public string Parti { get; set; }

        [Column(TypeName = "Nvarchar")]
        [StringLength(6)]
        public string Lot { get; set; }

        [Column(TypeName = "Nvarchar")]
        [Display(Name = "Önceki Adet")]
        public string OncekiAdet { get; set; }

        [Column(TypeName = "Nvarchar")]
        [Display(Name = "Sonraki Adet")]
        public string SonrakiAdet { get; set; }

        [Column(TypeName = "Nvarchar")]
        [StringLength(1)]
        [Display(Name = "Depo")]
        public string Depo { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(1)]
        [Display(Name = "Düzenlenecek")]
        public string Duzenlenecek { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(5)]
        [Display(Name = "Düzenlendi")]
        public string Duzenlendi { get; set; }

        [Column(TypeName = "Nvarchar")]
        [StringLength(1)]
        [Display(Name = "Durum")]
        public string Durum { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(100)]
        [Display(Name = "Kullanıcı")]
        public string Kullanici { get; set; }

        [Column(TypeName = "Nvarchar")]
        [StringLength(100)]
        [Display(Name = "Düzenleyen Kullanıcı")]
        public string DuzenleyenKullanici { get; set; }

        [Column(TypeName = "Nvarchar")]
        [StringLength(100)]
        [Display(Name = "Not")]
        public string Not { get; set; }

        public ICollection<Yonetici> Yoneticis { get; set; }
    }
}