using ClosedXML.Excel;
using MartUretimTakipMVC.Models.Database;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PagedList;

namespace MartUretimTakipMVC.Controllers
{
    public class DepoSayimController : Controller
    {
        // GET: DepoSayim
        Context db = new Context();

        [Authorize]
        public ActionResult Index(string filter, int sayfa = 1)
        {
            var topveri = db.DepoSayimlar.Count();
            ViewBag.ToplamSayim = topveri;
            var username = Session["YoneticiIsim"] as string;
            ViewBag.Kullanici = username;
            var deposayim = db.DepoSayimlar.Where(x => x.Durum == "A" && (x.Duzenlenecek == "T" || x.Duzenlenecek == "F")).OrderByDescending(x => x.KayitID).ToPagedList(sayfa, 100);
            return View(deposayim);
        }

        public ActionResult TersSiralama(int sayfa = 1)
        {
            var username = Session["YoneticiIsim"] as string;
            ViewBag.Kullanici = username;
            var topveri = db.DepoSayimlar.Count();
            ViewBag.ToplamSayim = topveri;
            var deposayim = db.DepoSayimlar.Where(x => x.Durum == "A" && (x.Duzenlenecek == "T" || x.Duzenlenecek == "F")).OrderBy(x => x.KayitID).ToPagedList(sayfa, 100);
            return View("Index", deposayim);
        }

        [HttpGet]
        public ActionResult StokEkle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StokEkle(DepoSayim ds)
        {
            try
            {
                string stokkodu = ds.QRData.Substring(0, 8);
                string stokkodu1 = ds.QRData.Substring(9, 8);
                string dolar1 = ds.QRData.Substring(8, 1);
                string dolar2 = ds.QRData.Substring(24, 1);
                //bool DIDsorgu = db.DepoSayimlar.Any(x => x.DID == ds.QRData.Substring(9, 15));
                var username = Session["YoneticiIsim"] as string;

                if (ds.QRData.Length <= 100 && stokkodu == stokkodu1 && dolar1 == dolar2 && dolar1 == "$" && dolar2 == "$") // Mart QR'ı mı sorgusu
                {
                    if (db.DepoSayimlar.Any(x => x.DID == ds.QRData.Substring(9, 15)) == false) // Girilen DID db de var mı sorgusu
                    {
                        db.DepoSayimlar.Add(ds);
                        ds.Durum = "A";
                        ds.Duzenlenecek = "F";
                        ds.EklenmeTarih = System.DateTime.Now.ToString("dd/MM/yyyy:HH:mm");
                        ds.StokKodu = ds.QRData.Substring(0, 8);
                        ds.DID = ds.QRData.Substring(9, 15);
                        ds.Parti = ds.QRData.Substring(17, 2);
                        ds.Lot = ds.QRData.Substring(19, 5);
                        ds.Duzenlendi = "Hayır";
                        ds.Kullanici = username;
                        ds.Not = "-";
                        ds.DuzenleyenKullanici = "-";
                        if (ds.Adet.StartsWith("0"))
                        {
                            ViewBag.Msg = "Adet bilgisi 0 ile başlayamaz";
                            return View("StokEklePopup");
                        }
                        else
                        {
                            if (ds.QRData[ds.QRData.Length - 2].ToString() == "$")
                            {
                                ds.Depo = ds.QRData[ds.QRData.Length - 1].ToString();
                                db.SaveChanges();
                                ViewBag.BasariMsg = "Stok Ekleme Başarılı";
                                return View("StokEklePopup");
                            }
                            else
                            {
                                ViewBag.Msg = "QR Kodu kontrol ediniz";
                                return View("StokEklePopup");
                            }
                        }
                    }
                    else
                    {
                        ViewBag.Msg = "Bu malzeme zaten eklenmiş";
                        return View("StokEklePopup");
                    }
                }
                else
                {
                    ViewBag.Msg = "Mart Elektronik'e ait QR kodu giriniz";
                    return View("StokEklePopup");
                }
            }
            catch
            {
                ViewBag.Msg = "Mart Elektronik'e ait QR kodu giriniz";
                return View("StokEklePopup");
            }
        }

        [Authorize(Roles = "3,2")]
        public ActionResult StokSil(int id, GecmisHareket gh)
        {
            var stok = db.DepoSayimlar.Find(id);
            db.DepoSayimlar.Remove(stok);
            db.GecmisHarekets.Add(gh);
            gh.Durum = "P";
            gh.EklenmeTarihi = stok.EklenmeTarih;
            gh.DuzenlenmeTarihi = stok.DuzenlenmeTarih;
            gh.StokKodu = stok.StokKodu;
            gh.DID = stok.DID;
            gh.Parti = stok.Parti;
            gh.Lot = stok.Lot;
            gh.OncekiAdet = stok.OncekiAdet;
            gh.SonrakiAdet = stok.Adet;
            gh.Depo = stok.Depo;
            gh.Duzenlenecek = stok.Duzenlenecek;
            gh.Duzenlendi = stok.Duzenlendi;
            gh.Kullanici = stok.Kullanici;
            gh.DuzenleyenKullanici = stok.DuzenleyenKullanici;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "3,2")]
        public ActionResult StokDuzenle(DepoSayim ds, GecmisHareket gh)
        {
            var username = Session["YoneticiIsim"] as string;
            var stok = db.DepoSayimlar.Find(ds.KayitID);
            if (ds.StokKodu.Length == 8 && ds.Parti.Length == 2 && ds.Lot.Length == 5 && ds.DID.Length == 15 && ds.Depo.Length == 1)
            {
                string stokkodu = ds.DID.Substring(0, 8);
                string parti = ds.DID.Substring(8, 2);
                string lot = ds.DID.Substring(10, 5);

                if (ds.StokKodu == stokkodu && ds.Parti == parti && ds.Lot == lot)
                {
                    stok.DuzenlenmeTarih = System.DateTime.Now.ToString("dd/MM/yyyy:HH:mm");
                    stok.StokKodu = ds.StokKodu;
                    stok.DID = ds.DID;
                    stok.Parti = ds.Parti;
                    stok.Lot = ds.Lot;
                    stok.OncekiAdet = ds.OncekiAdet;
                    stok.Adet = ds.Adet;
                    stok.Depo = ds.Depo;
                    stok.Duzenlenecek = "F";
                    stok.Duzenlendi = "Evet";
                    stok.Kullanici = stok.Kullanici;
                    stok.DuzenleyenKullanici = username;
                    stok.Not = "-";
                    db.GecmisHarekets.Add(gh);
                    gh.Durum = "A";
                    gh.EklenmeTarihi = stok.EklenmeTarih;
                    gh.DuzenlenmeTarihi = System.DateTime.Now.ToString("dd/MM/yyyy:HH:mm");
                    gh.StokKodu = ds.StokKodu;
                    gh.DID = ds.DID;
                    gh.OncekiAdet = stok.OncekiAdet;
                    gh.SonrakiAdet = ds.Adet;
                    gh.Depo = ds.Depo;
                    gh.Kullanici = ds.Kullanici;
                    gh.Duzenlenecek = "F";
                    gh.Duzenlendi = "Evet";
                    gh.Kullanici = stok.Kullanici;
                    gh.DuzenleyenKullanici = username;
                    gh.Not = "-";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Msg = "DID Bilgisi Hatalı";
                    return View("StokGetir", stok);
                }
            }
            else
            {
                ViewBag.Msg = "Girilen Bilgiler Hatalı";
                return View("StokGetir", stok);
            }
        }

        [Authorize(Roles = "3,2")]
        public ActionResult StokGetir(int id)
        {
            var stok = db.DepoSayimlar.Find(id);
            stok.QRData = stok.QRData;
            stok.OncekiAdet = stok.Adet;
            ViewBag.OncekiAdet = stok.OncekiAdet;
            stok.Adet = null;
            stok.Kullanici = stok.Kullanici;
            return View("StokGetir", stok);
        }

        public ActionResult Filtrele(int sayfa = 1)
        {
            var duzenlenecekler = db.DepoSayimlar.Where(x => x.Duzenlenecek == "T").Count();
            ViewBag.ToplamSayim = duzenlenecekler;
            var username = Session["YoneticiIsim"] as string;
            ViewBag.Kullanici = username;
            var duzenlenecek = db.DepoSayimlar.Where(x => x.Duzenlenecek == "T").OrderByDescending(x => x.KayitID).ToPagedList(sayfa, 100);
            return View("Index", duzenlenecek);
        }

        public ActionResult ExcelAktarma()
        {
            string tarih = System.DateTime.Now.ToString("dd.MM.yyyy_HH.mm");

            using (var workbook = new XLWorkbook())
            {
                var deposayimlist = db.DepoSayimlar.ToList();
                var ws = workbook.Worksheets.Add("DepoSayim");
                ws.Cell(1, 1).Value = "Eklenme Tarihi";
                ws.Cell(1, 2).Value = "Düzenlenme Tarihi";
                ws.Cell(1, 3).Value = "Stok Kodu";
                ws.Cell(1, 4).Value = "DID";
                ws.Cell(1, 5).Value = "Parti";
                ws.Cell(1, 6).Value = "Lot";
                ws.Cell(1, 7).Value = "Adet";
                ws.Cell(1, 8).Value = "Depo";
                ws.Cell(1, 9).Value = "Durum";
                ws.Cell(1, 10).Value = "Düzenlenecek";
                ws.Cell(1, 11).Value = "Düzenlendi";
                ws.Cell(1, 12).Value = "Kullanıcı";
                ws.Cell(1, 13).Value = "Düzenleyen";
                ws.Cell(1, 14).Value = "Not";
                int row = 2;
                foreach (var item in deposayimlist)
                {
                    ws.Cell(row, 1).Value = item.EklenmeTarih;
                    ws.Cell(row, 2).Value = item.DuzenlenmeTarih;
                    ws.Cell(row, 3).Value = item.StokKodu;
                    ws.Cell(row, 4).Value = item.DID;
                    ws.Cell(row, 5).Value = item.Parti;
                    ws.Cell(row, 6).Value = item.Lot;
                    ws.Cell(row, 7).Value = item.Adet;
                    ws.Cell(row, 8).Value = item.Depo;
                    ws.Cell(row, 9).Value = item.Durum;
                    ws.Cell(row, 10).Value = item.Duzenlenecek;
                    ws.Cell(row, 11).Value = item.Duzenlendi;
                    ws.Cell(row, 12).Value = item.Kullanici;
                    ws.Cell(row, 13).Value = item.DuzenleyenKullanici;
                    ws.Cell(row, 14).Value = item.Not;
                    row++;
                }
                var titles = ws.Cells("A1:M1").Style.Font.Bold = true;

                ws.Column(1).Width = 17; // 1. sütun
                ws.Column(2).Width = 17; // 2. sütun
                ws.Column(3).Width = 12; // 3. sütun
                ws.Column(4).Width = 17; // 4. sütun
                ws.Column(10).Width = 13; // 10. sütun
                ws.Column(11).Width = 13; // 11. sütun
                ws.Column(12).Width = 17; // 12. sütun
                ws.Column(13).Width = 17; // 12. sütun
                ws.Column(14).Width = 25; // 13. sütun

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DepoSayim (" + tarih + ").xlsx");
                }
            }
        }

        [HttpGet]
        public ActionResult StokEklePopup()
        {
            return View();
        }

        public ActionResult NotGetir(int id)
        {
            var stoknot = db.DepoSayimlar.Find(id);
            if (stoknot.Not == "-")
            {
                stoknot.Not = null;
            }
            else
            {
                stoknot.Not = stoknot.Not;
            }
            return View("NotGetir", stoknot);
        }

        public ActionResult NotEklePopup(DepoSayim ds, GecmisHareket gh)
        {
            var stoknot = db.DepoSayimlar.Find(ds.KayitID);
            if (stoknot.Not != ds.Not)
            {
                stoknot.Not = ds.Not;
                stoknot.Duzenlenecek = "T";
                ViewBag.Not = ds.Not;                
                db.GecmisHarekets.Add(gh);
                gh.EklenmeTarihi = stoknot.EklenmeTarih;
                gh.DuzenlenmeTarihi = stoknot.DuzenlenmeTarih;
                gh.StokKodu = stoknot.StokKodu;
                gh.DID = stoknot.DID;
                gh.Parti = stoknot.Parti;
                gh.Lot = stoknot.Lot;
                gh.OncekiAdet = stoknot.OncekiAdet;
                gh.SonrakiAdet = stoknot.Adet;
                gh.Depo = stoknot.Depo;
                gh.Durum = stoknot.Durum;
                gh.Duzenlenecek = "T";
                gh.Duzenlendi = stoknot.Duzenlendi;
                gh.Kullanici = stoknot.Kullanici;
                gh.Not = ds.Not;
                gh.Duzenlenecek = "T";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.EklenmisNot = "Bu not zaten eklenmiş";
                return View("NotGetir");
            }
        }

        public ActionResult Filter(string filter, int sayfa = 1)
        {
            var topveri = db.DepoSayimlar.Count();
            ViewBag.ToplamSayim = topveri;
            var username = Session["YoneticiIsim"] as string;
            ViewBag.Kullanici = username;

            var sayims = from s in db.DepoSayimlar
                         select s;
            if (!string.IsNullOrEmpty(filter))
            {
                sayims = sayims.Where(x => x.StokKodu.Equals(filter) || x.DID.Equals(filter));
            }
            return View("Index", sayims.OrderByDescending(x => x.KayitID).ToPagedList(sayfa, 100));
        }
    }
}