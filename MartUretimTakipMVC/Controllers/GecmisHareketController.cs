using ClosedXML.Excel;
using MartUretimTakipMVC.Models.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PagedList;

namespace MartUretimTakipMVC.Controllers
{
    public class GecmisHareketController : Controller
    {
        // GET: GecmisHareket
        Context db = new Context();

        [Authorize(Roles = "3,2")]
        public ActionResult Index(int sayfa = 1)
        {
            var topveri = db.GecmisHarekets.Count();
            ViewBag.ToplamHareket = topveri;
            var username = Session["YoneticiIsim"] as string;
            ViewBag.Kullanici = username;
            var gecmishareket = db.GecmisHarekets.OrderByDescending(x => x.HareketID).ToPagedList(sayfa, 100);
            return View(gecmishareket);
        }

        [Authorize(Roles = "3,2")]
        public ActionResult SayimGeriAl(GecmisHareket gh, DepoSayim ds, EtiketsizSayim es)
        {
            var sayim = db.GecmisHarekets.Find(gh.HareketID);
            if (sayim.Durum == "P")
            {
                if (ds.StokKodu.Length == 8 && ds.Parti.Length == 2 && ds.Lot.Length == 5 && ds.DID.Length == 15 && ds.Depo.Length == 1)
                {
                    string stokkodu = ds.DID.Substring(0, 8);
                    string parti = ds.DID.Substring(8, 2);
                    string lot = ds.DID.Substring(10, 5);
                    if (ds.StokKodu == stokkodu && ds.Parti == parti && ds.Lot == lot)
                    {
                        db.DepoSayimlar.Add(ds);
                        ds.EklenmeTarih = sayim.EklenmeTarihi;
                        ds.DuzenlenmeTarih = sayim.DuzenlenmeTarihi;
                        ds.StokKodu = sayim.StokKodu;
                        ds.DID = sayim.DID;
                        ds.Parti = gh.Parti;
                        ds.Lot = gh.Lot;
                        ds.OncekiAdet = sayim.OncekiAdet;
                        ds.Adet = gh.SonrakiAdet;
                        ds.Duzenlenecek = sayim.Duzenlenecek;
                        ds.Duzenlendi = sayim.Duzenlendi;
                        ds.Depo = sayim.Depo;
                        ds.Durum = "A";
                        ds.Kullanici = sayim.Kullanici;
                        ds.DuzenleyenKullanici = sayim.DuzenleyenKullanici;
                        db.GecmisHarekets.Remove(sayim);
                        db.SaveChanges();
                        return RedirectToAction("Index", "DepoSayim");
                    }
                    else
                    {
                        ViewBag.Msg = "DID Bilgisi Hatalı";
                        return View("SayimGetir", sayim);
                    }
                }
                else
                {
                    ViewBag.Msg = "Girilen Bilgiler Hatalı";
                    return View("SayimGetir", sayim);
                }
            }
            else
            {
                ViewBag.Msg = "Bu Malzeme Silinmemiş";
                return View("SayimGetir", sayim);
            }
        }

        [Authorize(Roles = "3,2")]
        public ActionResult SayimGetir(int id)
        {
            var sayim = db.GecmisHarekets.Find(id);
            if (sayim.DID != null)
            {
                return View("SayimGetir", sayim);
            }
            else
            {
                return View("EtiketsizSayimGetir", sayim);
            }
        }

        public ActionResult EtiketsizSayimGetir(int id)
        {
            var etiketsizsayim = db.GecmisHarekets.Find(id);
            return View("EtiketsizSayimGetir", etiketsizsayim);
        }

        public ActionResult EtiketsizSayimGeriAl(EtiketsizSayim es, GecmisHareket gh)
        {
            var sayim = db.GecmisHarekets.Find(gh.HareketID);
            if (sayim.Durum == "P")
            {
                db.EtiketsizSayims.Add(es);
                es.EklenmeTarih = sayim.EklenmeTarihi;
                es.DuzenlenmeTarih = sayim.DuzenlenmeTarihi;
                es.StokKodu = sayim.StokKodu;
                es.OncekiAdet = sayim.OncekiAdet;
                es.Adet = sayim.SonrakiAdet;
                es.Kullanici = sayim.Kullanici;
                es.DuzenleyenKullanici = sayim.DuzenleyenKullanici;
                es.Durum = "A";
                es.Duzenlendi = sayim.Duzenlendi;
                es.Duzenlenecek = sayim.Duzenlenecek;
                es.Not = sayim.Not;
                db.GecmisHarekets.Remove(sayim);
                db.SaveChanges();
                return RedirectToAction("Index", "EtiketsizSayim");
            }
            else
            {
                ViewBag.Msg = "Bu malzeme silinmemiş";
                return View("EtiketsizSayimGetir", sayim);
            }
        }

        [Authorize(Roles = "3,2")]
        public ActionResult SilinenSayimlar(int sayfa = 1)
        {
            var silinenveri = db.GecmisHarekets.Where(x => x.Durum == "P").Count();
            ViewBag.ToplamHareket = silinenveri;
            var username = Session["YoneticiIsim"] as string;
            ViewBag.Kullanici = username;
            var silinensayim = db.GecmisHarekets.Where(x => x.Durum == "P").ToList();
            return View("Index", silinensayim.OrderByDescending(x => x.HareketID).ToPagedList(sayfa, 100));
        }

        [Authorize(Roles = "3,2")]
        public ActionResult DuzenlenenSayimlar(int sayfa = 1)
        {
            var duzenlenen = db.GecmisHarekets.Where(x => x.Duzenlendi == "Evet").Count();
            ViewBag.ToplamHareket = duzenlenen;
            var username = Session["YoneticiIsim"] as string;
            ViewBag.Kullanici = username;
            var duzenlenenler = db.GecmisHarekets.Where(x => x.Duzenlendi == "Evet").ToList();
            return View("Index", duzenlenenler.OrderByDescending(x => x.HareketID).ToPagedList(sayfa, 100));
        }

        public ActionResult ExcelAktarma()
        {
            var topveri = db.GecmisHarekets.Count();
            ViewBag.ToplamHareket = topveri;
            string tarih = System.DateTime.Now.ToString("dd.MM.yyyy_HH.mm");

            using (var workbook = new XLWorkbook())
            {
                var gecmishareketlist = db.GecmisHarekets.ToList();
                var ws = workbook.Worksheets.Add("Geçmiş Hareketler");
                ws.Cell(1, 1).Value = "Eklenme Tarihi";
                ws.Cell(1, 2).Value = "Düzenlenme Tarihi";
                ws.Cell(1, 3).Value = "Stok Kodu";
                ws.Cell(1, 4).Value = "DID";
                ws.Cell(1, 5).Value = "Parti";
                ws.Cell(1, 6).Value = "Lot";
                ws.Cell(1, 7).Value = "Önceki Adet";
                ws.Cell(1, 8).Value = "Sonraki Adet";
                ws.Cell(1, 9).Value = "Depo";
                ws.Cell(1, 10).Value = "Durum";
                ws.Cell(1, 11).Value = "Düzenlenecek";
                ws.Cell(1, 12).Value = "Düzenlendi";
                ws.Cell(1, 13).Value = "Kullanıcı";
                ws.Cell(1, 14).Value = "Düzenleyen";
                ws.Cell(1, 15).Value = "Not";

                int row = 2;

                foreach (var item in gecmishareketlist)
                {
                    ws.Cell(row, 1).Value = item.EklenmeTarihi;
                    ws.Cell(row, 2).Value = item.DuzenlenmeTarihi;
                    ws.Cell(row, 3).Value = item.StokKodu;
                    ws.Cell(row, 4).Value = item.DID;
                    ws.Cell(row, 5).Value = item.Parti;
                    ws.Cell(row, 6).Value = item.Lot;
                    ws.Cell(row, 7).Value = item.OncekiAdet;
                    ws.Cell(row, 8).Value = item.SonrakiAdet;
                    ws.Cell(row, 9).Value = item.Depo;
                    ws.Cell(row, 10).Value = item.Durum;
                    ws.Cell(row, 11).Value = item.Duzenlenecek;
                    ws.Cell(row, 12).Value = item.Duzenlendi;
                    ws.Cell(row, 13).Value = item.Kullanici;
                    ws.Cell(row, 14).Value = item.DuzenleyenKullanici;
                    ws.Cell(row, 15).Value = item.Not;
                    row++;
                }

                var titles = ws.Cells("A1:N1").Style.Font.Bold = true;

                ws.Column(1).Width = 17; // 1. sütun
                ws.Column(2).Width = 17; // 2. sütun
                ws.Column(3).Width = 10; // 3. sütun
                ws.Column(4).Width = 17; // 4. sütun
                ws.Column(7).Width = 12; // 7. sütun
                ws.Column(8).Width = 12; // 8. sütun
                ws.Column(11).Width = 13; // 11. sütun
                ws.Column(12).Width = 13; // 12. sütun
                ws.Column(13).Width = 17; // 13. sütun
                ws.Column(14).Width = 17; // 13. sütun
                ws.Column(15).Width = 25; // 14. sütun


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "GecmisHareket (" + tarih + ").xlsx");
                }
            }
        }

        public ActionResult TersSiralama(int sayfa = 1)
        {
            var topveri = db.GecmisHarekets.Count();
            ViewBag.ToplamHareket = topveri;
            var gecmishareket = db.GecmisHarekets.OrderBy(x => x.HareketID).ToList();
            return View("Index", gecmishareket.OrderBy(x => x.HareketID).ToPagedList(sayfa, 100));
        }

        public ActionResult Filter(string filter, int sayfa = 1)
        {
            var topveri = db.GecmisHarekets.Count();
            ViewBag.ToplamHareket = topveri;
            var sayims = from s in db.GecmisHarekets
                         select s;
            if (!string.IsNullOrEmpty(filter))
            {
                sayims = sayims.Where(x => x.StokKodu.Equals(filter) || x.DID.Equals(filter));
            }
            return View("Index", sayims.OrderByDescending(x => x.HareketID).ToPagedList(sayfa, 100));
        }

        public ActionResult EtiketsizSayim(int sayfa = 1)
        {
            var topveri = db.GecmisHarekets.Where(x => x.DID == null).Count();
            ViewBag.ToplamHareket = topveri;
            var etiketsiz = db.GecmisHarekets.Where(x => x.DID == null).ToList();
            return View("Index", etiketsiz.OrderByDescending(x => x.HareketID).ToPagedList(sayfa, 100));
        }
    }
}