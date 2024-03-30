using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MartUretimTakipMVC.Models.Database
{
    public class Yonetici
    {
        [Key]
        public int KayıtID { get; set; }

        [Column(TypeName = "Nvarchar")]
        [StringLength(30)]
        [Display(Name = "Yönetici ID")]
        public string YoneticiID { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(100)]
        [Display(Name = "Yönetici")]
        public string YoneticiIsim { get; set; }

        [Column(TypeName = "Nvarchar")]
        [StringLength(50)]
        [Display(Name = "Yönetici Şifre")]
        public string YoneticiSifre { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(40)]
        [Display(Name = "Yönetici Görev")]
        public string YoneticiGorev { get; set; }

        [Column(TypeName = "Nvarchar")]
        [StringLength(1)]
        [Display(Name = "Yönetici Yetki")]
        public string YoneticiYetki { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(1)]
        [Display(Name = "Yönetici Durum")]
        public string YoneticiDurum { get; set; }

        //public ICollection<DepoSayim> DepoSayims { get; set; }
        public ICollection<GecmisHareket> GecmisHarekets { get; set; }
    }
}