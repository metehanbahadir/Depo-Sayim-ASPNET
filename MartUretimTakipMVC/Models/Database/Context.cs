using System.Data.Entity;

namespace MartUretimTakipMVC.Models.Database
{
    public class Context : DbContext
    {
        public DbSet<DepoSayim> DepoSayimlar { get; set; }
        public DbSet<Yonetici> Yoneticiler { get; set; }
        public DbSet<GecmisHareket> GecmisHarekets { get; set; }
        public DbSet<EtiketsizSayim> EtiketsizSayims { get; set; }
    }
}