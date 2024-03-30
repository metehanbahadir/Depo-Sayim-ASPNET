﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MartUretimTakipMVC.Models.Database
{
    public class DepoSayim
    {
        [Key]
        public int KayitID { get; set; }

        [Display(Name = "Eklenme Tarihi")]
        public string EklenmeTarih { get; set; }

        [Display(Name = "Düzenlenme Tarihi")]
        public string DuzenlenmeTarih { get; set; }

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
        [Display(Name = "Parti No")]
        public string Parti { get; set; }

        [Column(TypeName = "Nvarchar")]
        [StringLength(6)]
        [Display(Name = "Lot No")]
        public string Lot { get; set; }

        [Column(TypeName = "Nvarchar")]
        [Display(Name = "Önceki Adet")]
        public string OncekiAdet { get; set; }

        [Column(TypeName = "Nvarchar")]
        [Display(Name = "Adet")]
        public string Adet { get; set; }

        [Column(TypeName = "Nvarchar")]
        [StringLength(1)]
        [Display(Name = "Depo")]
        public string Depo { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(1)]
        [Display(Name = "Stok Durumu")]
        public string Durum { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(1)]
        public string Duzenlenecek { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(100)]
        [Display(Name = "İşlemi Yapan Personel")]
        public string Kullanici { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(100)]
        [Display(Name = "Düzenleyen Personel")]
        public string DuzenleyenKullanici { get; set; }

        [Column(TypeName = "Nvarchar")]
        [StringLength(35)]
        [Display(Name = "QR Kod")]
        public string QRData { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(5)]
        public string Duzenlendi { get; set; }

        [Column(TypeName = "Nvarchar")]
        [StringLength(100)]
        [Display(Name = "Not")]
        public string Not { get; set; }

        public ICollection<Yonetici> Yoneticis { get; set; }
    }
}