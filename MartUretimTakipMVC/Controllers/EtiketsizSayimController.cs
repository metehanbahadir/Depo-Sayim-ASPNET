using ClosedXML.Excel;
using MartUretimTakipMVC.Models.Database;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using PagedList;

namespace MartUretimTakipMVC.Controllers
{
    public class EtiketsizSayimController : Controller
    {
        // GET: EtiketsizSayim

        Context db = new Context();

        [Authorize]
        public ActionResult Index(string filter, int sayfa=1)
        {
            var topveri = db.EtiketsizSayims.Count();
            ViewBag.ToplamSayim = topveri;
            var username = Session["YoneticiIsim"] as string;
            ViewBag.Kullanici = username;
            var etiketsizsayim = db.EtiketsizSayims.OrderByDescending(x => x.KayitID).ToList().ToPagedList(sayfa,100);
            return View(etiketsizsayim);
        }

        public ActionResult TersSiralama(int sayfa = 1)
        {
            var username = Session["YoneticiIsim"] as string;
            ViewBag.Kullanici = username;
            var topveri = db.EtiketsizSayims.Count();
            ViewBag.ToplamSayim = topveri;
            var etiketsiz = db.EtiketsizSayims.Where(x => x.Durum == "A" && (x.Duzenlenecek == "T" || x.Duzenlenecek == "F")).OrderBy(x => x.KayitID).ToPagedList(sayfa,100);
            return View("Index", etiketsiz);
        }

        public ActionResult Duzenlenecek(int sayfa=1)
        {
            var topveri = db.EtiketsizSayims.Where(x => x.Duzenlenecek == "T").Count();
            ViewBag.ToplamSayim = topveri;
            var duzenlenecek = db.EtiketsizSayims.Where(x => x.Duzenlenecek == "T").OrderByDescending(x=>x.KayitID).ToPagedList(sayfa,100);
            return View("Index", duzenlenecek);
        }

        [HttpGet]
        public ActionResult EtiketsizSayimEkle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EtiketsizSayimEklePopup(EtiketsizSayim es)
        {
            //bool StokSorgu = db.EtiketsizSayims.Any(x => x.StokKodu == es.StokKodu);
            var username = Session["YoneticiIsim"] as string;
            db.EtiketsizSayims.Add(es);
            if (es.StokKodu.Length == 8)
            {
                if (db.EtiketsizSayims.Any(x => x.StokKodu == es.StokKodu) == false)
                {
                    if (!es.Adet.StartsWith("0"))
                    {
                        es.EklenmeTarih = System.DateTime.Now.ToString("dd/MM/yyyy:HH:mm");
                        es.Durum = "A";
                        es.Duzenlendi = "Hayır";
                        es.Duzenlenecek = "F";
                        es.Not = "-";
                        es.Kullanici = username;
                        db.SaveChanges();
                        ViewBag.BasariMsg = "Etiketsiz Sayım Ekleme Başarılı";
                        return View("EtiketsizSayimEkle");
                    }
                    else
                    {
                        ViewBag.Msg = "Adet 0 ile başlayamaz";
                        return View("EtiketsizSayimEkle");
                    }
                }
                else
                {
                    ViewBag.Msg = "Bu malzeme zaten eklenmiş";
                    return View("EtiketsizSayimEkle");
                }
            }
            else
            {
                ViewBag.Msg = "StokKodu bilgisini kontrol ediniz";
                return View("EtiketsizSayimEkle");
            }
        }

        [Authorize(Roles = "3,2")]
        public ActionResult EtiketsizSayimSil(int id, GecmisHareket gh)
        {
            var etiketsizsayim = db.EtiketsizSayims.Find(id);
            db.EtiketsizSayims.Remove(etiketsizsayim);
            db.GecmisHarekets.Add(gh);
            gh.EklenmeTarihi = etiketsizsayim.EklenmeTarih;
            gh.DuzenlenmeTarihi = etiketsizsayim.DuzenlenmeTarih;
            gh.StokKodu = etiketsizsayim.StokKodu;
            gh.OncekiAdet = etiketsizsayim.OncekiAdet;
            gh.SonrakiAdet = etiketsizsayim.Adet;
            gh.Duzenlendi = etiketsizsayim.Duzenlendi;
            gh.Duzenlenecek = etiketsizsayim.Duzenlenecek;
            gh.Kullanici = etiketsizsayim.Kullanici;
            gh.DuzenleyenKullanici = etiketsizsayim.DuzenleyenKullanici;
            gh.Durum = "P";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "3,2")]
        public ActionResult EtiketsizSayimDuzenle(EtiketsizSayim es, GecmisHareket gh)
        {
            var username = Session["YoneticiIsim"] as string;
            var etiketsiz = db.EtiketsizSayims.Find(es.KayitID);
            etiketsiz.EklenmeTarih = etiketsiz.EklenmeTarih;
            etiketsiz.DuzenlenmeTarih = System.DateTime.Now.ToString("dd/MM/yyyy:HH:mm");
            etiketsiz.StokKodu = es.StokKodu;
            etiketsiz.OncekiAdet = es.OncekiAdet;
            etiketsiz.Adet = es.Adet;
            etiketsiz.Duzenlenecek = "F";
            etiketsiz.Duzenlendi = "Evet";
            etiketsiz.Kullanici = etiketsiz.Kullanici;
            etiketsiz.DuzenleyenKullanici = username;
            etiketsiz.Not = "-";
            db.GecmisHarekets.Add(gh);
            gh.Durum = "A";
            gh.EklenmeTarihi = etiketsiz.EklenmeTarih;
            gh.DuzenlenmeTarihi = System.DateTime.Now.ToString("dd/MM/yyyy:HH:mm");
            gh.StokKodu = es.StokKodu;
            gh.OncekiAdet = es.OncekiAdet;
            gh.SonrakiAdet = es.Adet;
            gh.Kullanici = etiketsiz.Kullanici;
            gh.Duzenlenecek = "F";
            gh.Duzenlendi = "Evet";
            gh.DuzenleyenKullanici = username;
            gh.Not = etiketsiz.Not;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "3,2")]
        public ActionResult EtiketsizSayimGetir(int id)
        {
            var etiketsiz = db.EtiketsizSayims.Find(id);
            etiketsiz.EklenmeTarih = etiketsiz.EklenmeTarih;
            etiketsiz.DuzenlenmeTarih = etiketsiz.DuzenlenmeTarih;
            etiketsiz.StokKodu = etiketsiz.StokKodu;
            etiketsiz.OncekiAdet = etiketsiz.Adet;
            etiketsiz.Adet = null;
            etiketsiz.Kullanici = etiketsiz.Kullanici;
            ViewBag.OncekiAdet = etiketsiz.OncekiAdet;
            return View("EtiketsizSayimGetir", etiketsiz);
        }

        public ActionResult NotGetir(int id)
        {
            var etiketsiz = db.EtiketsizSayims.Find(id);
            if (etiketsiz.Not == "-")
            {
                etiketsiz.Not = null;
            }
            else
            {
                etiketsiz.Not = etiketsiz.Not;
            }
            return View("NotGetir", etiketsiz);
        }

        public ActionResult NotEklePopup(EtiketsizSayim es, GecmisHareket gh)
        {
            var username = Session["YoneticiIsim"] as string;
            var etiketsiz = db.EtiketsizSayims.Find(es.KayitID);
            etiketsiz.Not = es.Not;
            etiketsiz.Duzenlenecek = "T";
            db.GecmisHarekets.Add(gh);
            gh.EklenmeTarihi = etiketsiz.EklenmeTarih;
            gh.DuzenlenmeTarihi = etiketsiz.DuzenlenmeTarih;
            gh.StokKodu = etiketsiz.StokKodu;
            gh.OncekiAdet = etiketsiz.OncekiAdet;
            gh.SonrakiAdet = etiketsiz.Adet;
            gh.Durum = etiketsiz.Durum;
            gh.Duzenlenecek = "T";
            gh.Duzenlendi = etiketsiz.Duzenlendi;
            gh.Kullanici = etiketsiz.Kullanici;
            gh.DuzenleyenKullanici = username;
            gh.Not = es.Not;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ExcelAktarma()
        {
            string tarih = System.DateTime.Now.ToString("dd.MM.yyyy_HH.mm");

            using (var workbook = new XLWorkbook())
            {
                var etiketsizlist = db.EtiketsizSayims.ToList();
                var ws = workbook.Worksheets.Add("EtiketsizSayim");
                ws.Cell(1, 1).Value = "Eklenme Tarihi";
                ws.Cell(1, 2).Value = "Düzenlenme Tarihi";
                ws.Cell(1, 3).Value = "Stok Kodu";
                ws.Cell(1, 4).Value = "Adet";
                ws.Cell(1, 5).Value = "Durum";
                ws.Cell(1, 6).Value = "Düzenlenecek";
                ws.Cell(1, 7).Value = "Düzenlendi";
                ws.Cell(1, 8).Value = "Kullanıcı";
                ws.Cell(1, 9).Value = "Düzenleyen";
                ws.Cell(1, 10).Value = "Not";
                int row = 2;
                foreach (var item in etiketsizlist)
                {
                    ws.Cell(row, 1).Value = item.EklenmeTarih;
                    ws.Cell(row, 2).Value = item.DuzenlenmeTarih;
                    ws.Cell(row, 3).Value = item.StokKodu;
                    ws.Cell(row, 4).Value = item.Adet;
                    ws.Cell(row, 5).Value = item.Durum;
                    ws.Cell(row, 6).Value = item.Duzenlenecek;
                    ws.Cell(row, 7).Value = item.Duzenlendi;
                    ws.Cell(row, 8).Value = item.Kullanici;
                    ws.Cell(row, 9).Value = item.DuzenleyenKullanici;
                    ws.Cell(row, 10).Value = item.Not;
                    row++;
                }
                var titles = ws.Cells("A1:J1").Style.Font.Bold = true;

                ws.Column(1).Width = 17; // 1. sütun
                ws.Column(2).Width = 17; // 2. sütun
                ws.Column(3).Width = 12; // 3. sütun
                ws.Column(4).Width = 17; // 4. sütun
                ws.Column(6).Width = 13; // 10. sütun
                ws.Column(7).Width = 13; // 11. sütun
                ws.Column(8).Width = 17; // 12. sütun
                ws.Column(9).Width = 17; // 12. sütun
                ws.Column(10).Width = 25; // 13. sütun

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EtiketsizSayim (" + tarih + ").xlsx");
                }
            }
        }

        public ActionResult Filter(string filter, int sayfa=1)
        {
            var topveri = db.EtiketsizSayims.Count();
            ViewBag.ToplamSayim = topveri;
            var username = Session["YoneticiIsim"] as string;
            ViewBag.Kullanici = username;

            var sayims = from s in db.EtiketsizSayims
                         select s;
            if (!string.IsNullOrEmpty(filter))
            {
                sayims = sayims.Where(x => x.StokKodu.Equals(filter));
            }
            return View("Index", sayims.OrderByDescending(x=>x.KayitID).ToPagedList(sayfa,100));
        }
    }
}