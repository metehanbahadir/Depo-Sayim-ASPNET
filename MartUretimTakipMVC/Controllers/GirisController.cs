using MartUretimTakipMVC.Models.Database;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace MartUretimTakipMVC.Controllers
{
    public class GirisController : Controller
    {
        // GET: Giris
        Context db = new Context();

        [HttpGet]
        public ActionResult KullaniciGirisi()
        {
            return View();
        }

        [HttpPost]
        public ActionResult KullaniciGirisi(Yonetici y)
        {
            var user = db.Yoneticiler.FirstOrDefault(x => x.YoneticiID == y.YoneticiID && x.YoneticiSifre == y.YoneticiSifre);

            if (user != null)
            {
                Session["YoneticiIsim"] = user.YoneticiIsim;
                FormsAuthentication.SetAuthCookie(user.YoneticiID, false);
                Session["YoneticiID"] = user.YoneticiID;
                return RedirectToAction("Index", "DepoSayim");
            }
            else
            {
                ViewBag.BasarisizGiris = "Hatalı Giriş Bilgileri !";
                return View("KullaniciGirisiPopup");
            }
        }

        public ActionResult CikisYap()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("KullaniciGirisiPopup", "Giris");
        }

        public ActionResult KullaniciGirisiPopup()
        {
            return View();
        }
    }
}