using DocumentFormat.OpenXml.EMMA;
using MartUretimTakipMVC.Models.Database;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace MartUretimTakipMVC.Controllers
{
    public class KullaniciController : Controller
    {
        // GET: Kullanici
        Context db = new Context();

        [Authorize(Roles = "3,2")]
        public ActionResult Index()
        {
            var topkullanici = db.Yoneticiler.Count();
            ViewBag.ToplamKullanici = topkullanici;
            var kullanici = db.Yoneticiler.Where(x => x.YoneticiDurum == "A").ToList();
            return View(kullanici);
        }

        [Authorize(Roles = "3,2")]
        [HttpGet]
        public ActionResult KullaniciEkle()
        {
            return View();
        }

        [Authorize(Roles = "3,2")]
        [HttpPost]
        public ActionResult KullaniciEkle(Yonetici y)
        {
            db.Yoneticiler.Add(y);
            y.YoneticiDurum = "A";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "3,2")]
        public ActionResult KullaniciGetir(int id)
        {
            var kullanici = db.Yoneticiler.Find(id);
            return View("KullaniciGetir", kullanici);
        }

        public ActionResult KullaniciDuzenle(Yonetici y)
        {            
            var kullanici = db.Yoneticiler.Find(y.KayıtID);
            kullanici.YoneticiID = y.YoneticiID;
            kullanici.YoneticiSifre = y.YoneticiSifre;
            kullanici.YoneticiYetki = y.YoneticiYetki;
            kullanici.YoneticiGorev = y.YoneticiGorev;
            kullanici.YoneticiIsim = y.YoneticiIsim;
            db.SaveChanges();           
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "3")]
        public ActionResult KullaniciSil(int id)
        {
            var kullanici = db.Yoneticiler.Find(id);
            kullanici.YoneticiDurum = "P";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}