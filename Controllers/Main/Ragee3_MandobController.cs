using Warehouse.Models;
using Warehouse.Models.Administration;
using Warehouse.Models.Search;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;

using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Warehouse.GlobalClass;
using Warehouse.GlobalClass.Reports;
using Warehouse.Reports.Invoices;
using DevExpress.XtraPrinting;

using Warehouse.Models.Reports;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Warehouse.CustomFilters;
using Warehouse.Reports.Eradat;

namespace Warehouse.Controllers.Main
{
    public class Ragee3_MandobController : Controller
    {
        ConvertToEasternArabicNumerals ConvertToEasternArabicNumerals = new ConvertToEasternArabicNumerals();
        DB_StoreEntities db = new DB_StoreEntities();
        string[] allFormats = { "yyyy/MM/dd", "yyyy/M/d", "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy", "d/MM/yyyy", "yyyy-MM-dd", "yyyy-M-d", "dd-MM-yyyy", "d-M-yyyy", "dd-M-yyyy", "d-MM-yyyy", "yyyy MM dd", "yyyy M d", "dd MM yyyy", "d M yyyy", "dd M yyyy", "d MM yyyy" };
        ErrorViewModel Error = new ErrorViewModel();
        private int recordsPerPage = 300;
        int viewid = 2474;// حركة بيع

        //جلب الشركات لفاتورة العميل----------------------
        [HttpGet]
        public ActionResult GetMandob()
        {
            return PartialView();
        }
        //----------------------------------------
        [HttpGet]
        public ActionResult getAllMandob(int? page, string Search)
        {
            int? skipRecords = (page != null ? page.Value : 0) * recordsPerPage;

            var AllRecords = db.Mandobs
              .Where(s => string.IsNullOrEmpty(Search) ? true : s.Name.Contains(Search))
              .OrderBy(s => s.ID).Skip(skipRecords != null ? skipRecords.Value : 0)
              .Take(recordsPerPage).Select(a => new Cls_Mandob
              {
                  ID = a.ID,
                  Name = a.Name,
                  Sejil = a.Sejil,
                  JawalNO = a.JawalNO,
                  Address = a.Address
              }).ToList();
            var list = JsonConvert.SerializeObject(AllRecords, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
            //return Json(AllRecords, JsonRequestBehavior.AllowGet);
        }

        //************************************************************************************************************
        //*************************الفاتورة**************************************************************************
        //************************************************************************************************************
        [CustomAuthorize]
        [HttpGet]
        public ActionResult Ragee3_Mandob()
        {
            return View();
        }

        [HttpGet]
        public ActionResult loadInvoice(int id)
        {
            Cls_Invoice_Mandob Cls_Invoice_Mandob = new Cls_Invoice_Mandob();
            Invoice_Mandob Invoice_Mandob = db.Invoice_Mandob.Find(id);
            Cls_Invoice_Mandob.ID = Invoice_Mandob.ID;
            Cls_Invoice_Mandob.Invoice_Number = Invoice_Mandob.Invoice_Number;
            Cls_Invoice_Mandob.Customer_Type = Invoice_Mandob.Customer_Type;
            if (Invoice_Mandob.Customer_Type == 1)
            {
                Cls_Invoice_Mandob.Customer_Name = "مندوب";
            }
            else if (Invoice_Mandob.Customer_Type == 2)
            {
                Cls_Invoice_Mandob.Customer_Name = "محل";
            }
            else if (Invoice_Mandob.Customer_Type == 3)
            {
                Cls_Invoice_Mandob.Customer_Name = "منزل";
            }


            Cls_Invoice_Mandob.Mandob_id = Invoice_Mandob.Mandob_id;
            Cls_Invoice_Mandob.Mandob_Name = (Invoice_Mandob.Mandob_id != null) ? Invoice_Mandob.Mandob.Name : "";
            System.Globalization.GregorianCalendar GregorianCalendar = new GregorianCalendar();
            CultureInfo CultureInfo = new CultureInfo("en-US")
            {
                DateTimeFormat = { Calendar = GregorianCalendar }
            };
            Cls_Invoice_Mandob.Date_Invoice = Invoice_Mandob.Date_Invoice.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.GetCultureInfo("en-us", "en"));
            //Cls_Invoice_Mandob.Date_Invoice = Invoice_Mandob.Date_Invoice.ToString("yyyy-MM-dd");
            Cls_Invoice_Mandob.Date_Invoice_Hijri = Invoice_Mandob.Date_Invoice_Hijri;
            Cls_Invoice_Mandob.Price = Invoice_Mandob.Price;
            Cls_Invoice_Mandob.Total_Sadad = Invoice_Mandob.Total_Sadad;
            Cls_Invoice_Mandob.User_ID = Invoice_Mandob.User_ID;
            Cls_Invoice_Mandob.ComputerName = Invoice_Mandob.ComputerName;
            Cls_Invoice_Mandob.ComputerUser = Invoice_Mandob.ComputerUser;
            Cls_Invoice_Mandob.InDate = Invoice_Mandob.InDate.ToString("yyyy-MM-dd");
            //Cls_Invoice_Mandob.ClsInvoiceMandob_Product = new List<ClsInvoiceMandob_Product>();

            List<Invoice_Mandob_Sadad_Rproduct> Invoice_Mandob_Sadad_Rproduct = db.Invoice_Mandob_Sadad_Rproduct.Where(a => a.Invoice_Mandob_Sadad.Invoice_Mandob.ID == id).ToList();
            foreach (var item in Invoice_Mandob_Sadad_Rproduct)
            {
                Cls_Invoice_Mandob.ClsInvoiceMandob_Product.Add(
                    new ClsInvoiceMandob_Product
                    {
                        ID = item.ID,
                        Invoice_Mandob_Sadad_id = item.Invoice_Mandob_Sadad_id,
                        Return_Invoice_Product_id = item.Return_Invoice_Product_id,
                        Product_Name = item.Product.Name,
                        Amount = item.Amount,
                        Price = item.Price,
                        InDate = item.InDate//item.Taxes,
                    }
                    );
                Cls_Invoice_Mandob.Omola_Amount_AllProducts += item.Amount;
            }
            Omola Omola = db.Omolas.FirstOrDefault();
            Cls_Invoice_Mandob.Omola_Type = Omola.Omola_Type;
            Cls_Invoice_Mandob.Omola_quota = Omola.Omola_quota;
            if (Omola.Omola_Type == false)
            {
                Cls_Invoice_Mandob.Omola_Text = Omola.Omola_quota + " ريال لكل صنف ";
                Cls_Invoice_Mandob.Omola_Money_Orignal = Cls_Invoice_Mandob.Omola_Amount_AllProducts * Omola.Omola_quota;
            }
            else
            {
                Cls_Invoice_Mandob.Omola_Text = Omola.Omola_quota + " % " + " نسبة على كل صنف";
                Cls_Invoice_Mandob.Omola_Money_Orignal = ((Omola.Omola_quota / 100) * Cls_Invoice_Mandob.Price) + Cls_Invoice_Mandob.Price;

            }
            Cls_Invoice_Mandob.Omola_Money = Invoice_Mandob.Omola_Money ?? default(decimal);
            var list = JsonConvert.SerializeObject(Cls_Invoice_Mandob, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });

            return Content(list, "application/json");
        }


        [CustomAuthorize]
        [HttpPost]
        public ActionResult AddItem(int index, ClsInvoice_Mandob_Sadad ClsInvoice_Mandob_Sadad)
        {
            DateTime Date_Invoice = DateTime.Now;
            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            

         
            if (ClsInvoice_Mandob_Sadad.Invoice_Id == 0)
            {

                //**************************************************************
               int Sadad_ID  = Save_Sadad(ClsInvoice_Mandob_Sadad);
                //**************************************************************


                foreach (var item in ClsInvoice_Mandob_Sadad.ClsInvoice_Mandob_Sadad_Rproduct)
                {
                    Invoice_Mandob_Sadad_Rproduct Invoice_Mandob_Sadad_Rproduct = new Invoice_Mandob_Sadad_Rproduct
                    {
                        Invoice_Mandob_Sadad_id = Sadad_ID,

                        Return_Invoice_Product_id = item.Return_Invoice_Product_id,
                        Amount = item.Amount,
                        Price = item.Price,

                        InDate = Date_Invoice
                    };
                    db.Invoice_Mandob_Sadad_Rproduct.Add(Invoice_Mandob_Sadad_Rproduct);
                    db.SaveChanges();
                }
               
              
            }
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = ClsInvoice_Mandob_Sadad.Invoice_Id;
            //Error.Invoice_Number = Invoice_Number;
            //Error.Invoice_Product_ID = Invoice_Product_ID;
            //Error.Total_Sadad = Cls_Invoice_Mandob.Total_Sadad;
            Error.index = index;
            
            return Json(Error, JsonRequestBehavior.AllowGet);

        }


        [CustomAuthorize]
        [HttpPost]
        public ActionResult EditItem(int index, int Invoice_Mandob_Product_ID, ClsInvoice_Mandob_Sadad ClsInvoice_Mandob_Sadad)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            System.Globalization.DateTimeFormatInfo HijriDTFI;
            HijriDTFI = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
            HijriDTFI.Calendar = new System.Globalization.HijriCalendar();
            HijriDTFI.ShortDatePattern = "dd/MM/yyyy";

       
            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();

            //**************************************************************
            int Sadad_ID = Edit_Sadad(ClsInvoice_Mandob_Sadad);
            //**************************************************************
           
           
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = ClsInvoice_Mandob_Sadad.Invoice_Id;
            //Error.Invoice_Number = Invoice_Number;
            //Error.Invoice_Product_ID = Invoice_Product_ID;
            //Error.Total_Sadad = Cls_Invoice_Mandob.Total_Sadad;
            Error.index = index;

            return Json(Error, JsonRequestBehavior.AllowGet);

        }

        public int Save_Sadad(ClsInvoice_Mandob_Sadad ClsInvoice_Mandob_Sadad)
        {
            DateTime Date_Invoice = DateTime.Now;
            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            int _id = db.Invoice_Mandob_Sadad.Select(a => a.ID).DefaultIfEmpty(0).Max() + 1;
            //Add New
            Invoice_Mandob_Sadad Sadad = new Invoice_Mandob_Sadad
            {
                ID = _id,
                Invoice_Id = ClsInvoice_Mandob_Sadad.Invoice_Id,
                Sadad_Type_Id = 4, // ارجاع اصناف
                Money = ClsInvoice_Mandob_Sadad.Money,
              
                Date_Added = Date_Invoice,
                User_ID = UserID,
                ComputerName = computerDetails[0],
                ComputerUser = computerDetails[1],
                InDate = Date_Invoice
            };
            db.Invoice_Mandob_Sadad.Add(Sadad);
            db.SaveChanges();
            int Invoice_Id = ClsInvoice_Mandob_Sadad.Invoice_Id;
            Invoice_Mandob Invoice_Mandob = db.Invoice_Mandob.Find(Invoice_Id);
            decimal Total_Sadad = db.Invoice_Mandob_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();
            decimal Remain = Invoice_Mandob.Price - Total_Sadad;

            //*********************************************************************
            Invoice_Mandob.Total_Sadad = Total_Sadad;
            db.Entry(Invoice_Mandob).State = EntityState.Modified;
            db.SaveChanges();
            //*********************************************************************
            int Invoice_Number = db.Masrofats.Where(a => a.Is_Deleted == false).Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
            Masrofat Masrofat = new Masrofat
            {
                Invoice_Number = Invoice_Number,
                Invoice_Mandob_Sadad_ID = Sadad.ID,
                Date_Invoice = Date_Invoice,
                Masrofat_Type_Id = 4, //ارجاع اصناف
                Money = ClsInvoice_Mandob_Sadad.Money,
                Bian = "ارجاع الاصناف لفاتورة مبيعات رقم  " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Mandob.Invoice_Number.ToString())) + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob.Date_Invoice.ToString("yyyy/MM/dd"))
                   + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", ClsInvoice_Mandob_Sadad.Money))
                   + " ريال ",
                Userid_In = UserID,
                ComputerName = computerDetails[0],
                ComputerUser = computerDetails[1],
                InDate = Date_Invoice
            };
            db.Masrofats.Add(Masrofat);
            db.SaveChanges();
            //*********************************************************************
            return Sadad.ID;
        }

        public int Edit_Sadad(ClsInvoice_Mandob_Sadad ClsInvoice_Mandob_Sadad)
        {
            DateTime Date_Invoice = DateTime.Now;
            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            //Add New
            int Invoice_Id = ClsInvoice_Mandob_Sadad.Invoice_Id;
            Invoice_Mandob_Sadad Invoice_Mandob_Sadad = db.Invoice_Mandob_Sadad.Where(a => a.Invoice_Id == Invoice_Id).FirstOrDefault();
            Invoice_Mandob_Sadad.Sadad_Type_Id = 4; // ارجاع اصناف
            Invoice_Mandob_Sadad.Date_Added = Date_Invoice;
            Invoice_Mandob_Sadad.Money = ClsInvoice_Mandob_Sadad.Money;
            db.Entry(Invoice_Mandob_Sadad).State = EntityState.Modified;
            db.SaveChanges();

            Invoice_Mandob Invoice_Mandob = db.Invoice_Mandob.Find(Invoice_Id);
            decimal Total_Sadad = db.Invoice_Mandob_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();
            decimal Remain = Invoice_Mandob.Price - Total_Sadad;
            //*********************************************************************
            Invoice_Mandob.Total_Sadad = Total_Sadad;
            db.Entry(Invoice_Mandob).State = EntityState.Modified;
            db.SaveChanges();
            //*********************************************************************
            Masrofat Masrofat = db.Masrofats.Where(a => a.Invoice_Mandob_Sadad_ID == Invoice_Mandob_Sadad.ID).FirstOrDefault();

            if (Masrofat != null)
            {

                Masrofat.Date_Invoice = Date_Invoice;
                Masrofat.Masrofat_Type_Id = 4; //ارجاع اصناف
                Masrofat.Money = ClsInvoice_Mandob_Sadad.Money;
                    Masrofat.Bian = "ارجاع الاصناف لفاتورة مبيعات رقم  " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Mandob.Invoice_Number.ToString())) + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob.Date_Invoice.ToString("yyyy/MM/dd"))
                     + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", ClsInvoice_Mandob_Sadad.Money))
                     + " ريال ";
                Masrofat.Userid_In = UserID;
                Masrofat.ComputerName = computerDetails[0];
                Masrofat.ComputerUser = computerDetails[1];
                Masrofat.EditDate = Date_Invoice;
                db.Entry(Masrofat).State = EntityState.Modified;
                db.SaveChanges();

            }
            else
            {
                int Invoice_Number = db.Masrofats.Where(a => a.Is_Deleted == false).Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
                Masrofat = new Masrofat
                {
                    Invoice_Number = Invoice_Number,
                    Invoice_Mandob_Sadad_ID = ClsInvoice_Mandob_Sadad.ID,
                    Date_Invoice = Date_Invoice,
                    Masrofat_Type_Id = 4, //ارجاع اصناف
                    Money = ClsInvoice_Mandob_Sadad.Money,
                    Bian = "ارجاع الاصناف لفاتورة مبيعات رقم  " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Mandob.Invoice_Number.ToString())) + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob.Date_Invoice.ToString("yyyy/MM/dd"))
                       + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", ClsInvoice_Mandob_Sadad.Money))
                       + " ريال ",
                    Userid_In = UserID,
                    ComputerName = computerDetails[0],
                    ComputerUser = computerDetails[1],
                    InDate = Date_Invoice
                };
                db.Masrofats.Add(Masrofat);
                db.SaveChanges();
            }
            return Invoice_Mandob_Sadad.ID;

        }
        [CustomAuthorize]
        [HttpPost]
        public ActionResult EditItemForMandob(Cls_Invoice_Mandob Cls_Invoice_Mandob)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            System.Globalization.DateTimeFormatInfo HijriDTFI;
            HijriDTFI = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
            HijriDTFI.Calendar = new System.Globalization.HijriCalendar();
            HijriDTFI.ShortDatePattern = "dd/MM/yyyy";

            DateTime Date_Invoice = DateTime.ParseExact(Cls_Invoice_Mandob.Date_Invoice, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string Date_Invoice_Hijri = Date_Invoice.Date.ToString("dd/MM/yyyy", HijriDTFI);

            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();

            ///Edit

            Invoice_Mandob Invoice_Mandob = db.Invoice_Mandob.Find(Cls_Invoice_Mandob.ID);

            //*********************************************************************
            Invoice_Mandob.Mandob_id = Cls_Invoice_Mandob.Mandob_id;
            //*********************************************************************

            Invoice_Mandob.Date_Invoice = Date_Invoice;
            Invoice_Mandob.Date_Invoice_Hijri = Date_Invoice_Hijri;
            Invoice_Mandob.Price = Cls_Invoice_Mandob.Price;
            Invoice_Mandob.Taxes = 0;
            Invoice_Mandob.Payment_Type = Cls_Invoice_Mandob.Payment_Type;
            //*********************************************************************
            int Invoice_Id = Cls_Invoice_Mandob.ID;
            decimal Total_Sadad = db.Invoice_Mandob_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();
            Invoice_Mandob.Total_Sadad = Total_Sadad;
            //*********************************************************************
            Invoice_Mandob.User_ID = UserID;
            Invoice_Mandob.ComputerName = computerDetails[0];
            Invoice_Mandob.ComputerUser = computerDetails[1];
            Invoice_Mandob.InDate = DateTime.Now;

            db.Entry(Invoice_Mandob).State = EntityState.Modified;
            db.SaveChanges();




            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = Invoice_Mandob.ID;
            return Json(Error, JsonRequestBehavior.AllowGet);

        }
        [CustomAuthorize]
        [HttpPost]
        public ActionResult DeleteItem(int Invoice_Mandob_Product_ID, Cls_Invoice_Mandob Cls_Invoice_Mandob)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            System.Globalization.DateTimeFormatInfo HijriDTFI;
            HijriDTFI = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
            HijriDTFI.Calendar = new System.Globalization.HijriCalendar();
            HijriDTFI.ShortDatePattern = "dd/MM/yyyy";
            DateTime Date_Invoice = DateTime.ParseExact(Cls_Invoice_Mandob.Date_Invoice, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string Date_Invoice_Hijri = Date_Invoice.Date.ToString("dd/MM/yyyy", HijriDTFI);


            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();

            ///Edit

            Invoice_Mandob Invoice_Mandob = db.Invoice_Mandob.Find(Cls_Invoice_Mandob.ID);
            Invoice_Mandob.Mandob_id = Cls_Invoice_Mandob.Mandob_id;
            Invoice_Mandob.Date_Invoice = Date_Invoice;
            Invoice_Mandob.Date_Invoice_Hijri = Date_Invoice_Hijri;
            Invoice_Mandob.Price = Cls_Invoice_Mandob.Price;
            //*********************************************************************
            int Invoice_Id = Cls_Invoice_Mandob.ID;
            decimal Total_Sadad = db.Invoice_Mandob_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();
            Invoice_Mandob.Total_Sadad = Total_Sadad;
            //*********************************************************************
            Invoice_Mandob.User_ID = UserID;
            Invoice_Mandob.ComputerName = computerDetails[0];
            Invoice_Mandob.ComputerUser = computerDetails[1];
            Invoice_Mandob.InDate = DateTime.Now;

            db.Entry(Invoice_Mandob).State = EntityState.Modified;
            db.SaveChanges();

            Invoice_Mandob_Product Invoice_Mandob_Product = db.Invoice_Mandob_Product.Find(Invoice_Mandob_Product_ID);


            db.Invoice_Mandob_Product.Remove(Invoice_Mandob_Product);
            db.SaveChanges();


            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = Invoice_Mandob.ID;
            //Error.Invoice_Mandob_Product_ID = Invoice_Mandob_Product.ID;
            //Error.index = index;
            return Json(Error, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult DeleteInvoice(int ID)
        {

            Invoice_Mandob Invoice_Mandob = db.Invoice_Mandob.Find(ID);
            Invoice_Mandob.Is_Deleted = true;

            db.Entry(Invoice_Mandob).State = EntityState.Modified;
            db.SaveChanges();

            Error.ErrorName = "تم الحذف بنجاح ... جاري إعادة تحميل الصفحة";
            //Error.Invoice_Mandob_Product_ID = Invoice_Mandob_Product.ID;
            //Error.index = index;
            return Json(Error, JsonRequestBehavior.AllowGet);

        }

        //[HttpGet]
        //public ActionResult PrintInvoiceDailog(decimal id)
        //{
        //    ViewBag.MyUrl = "/Invoice_Mandob/PdfInvoice/" + id.ToString();
        //    return PartialView("PrintInvoice");
        //}
        [HttpGet]
        public ActionResult PdfInvoice(int id)
        {

            List<Prnt_Invoice_Mandob> List = new List<Prnt_Invoice_Mandob>();
            Invoice_Mandob Invoice_Mandob = db.Invoice_Mandob.Find(id);
            string TotalPrice_Tafkeet = new ToWord(Invoice_Mandob.Price, new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia)).ConvertToArabic();

            List = Invoice_Mandob.Invoice_Mandob_Product.Select(a => new Prnt_Invoice_Mandob
            {
                Warehouse_Address = "الحدود الشمالية - محافظة رفحاء - طريق عمر بن الخطاب - حي الروضة",
                Warehouse_Mobile = ConvertToEasternArabicNumerals.ConvertAR("0553390093"),
                Warehouse_Email = "naqirafha@gmail.com",
                Product_Name = ConvertToEasternArabicNumerals.ConvertAR(a.Product_Name),
                Amount = ConvertToEasternArabicNumerals.ConvertAR(a.Amount.ToString()),
                Price = ConvertToEasternArabicNumerals.ConvertAR(a.Price.ToString()),
                TotalPrice = ConvertToEasternArabicNumerals.ConvertAR(a.TotalPrice.ToString()),
                ID = ConvertToEasternArabicNumerals.ConvertAR(a.Invoice_Mandob.ID.ToString()),
                Mandob_id = Invoice_Mandob.Mandob_id,
                Mandob_Name = (Invoice_Mandob.Mandob_id != null) ? Invoice_Mandob.Mandob.Name : "",
                Mandob_Mobile = ConvertToEasternArabicNumerals.ConvertAR(a.Invoice_Mandob.Mandob.JawalNO),
                Date_Invoice = Invoice_Mandob.Date_Invoice.ToString("yyyy-mm-dd"),
                Date_Invoice_Hijri = Invoice_Mandob.Date_Invoice_Hijri,
                Price_Invoice = ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob.Price.ToString()),
                TotalPrice_Tafkeet = TotalPrice_Tafkeet
            }).ToList();

            Rpt_Invoice_Mandob report = new Rpt_Invoice_Mandob();
            report.DataSource = List;
            //string _path = System.Web.HttpContext.Current.Server.MapPath(@"~/Reports/pdf");
            //string reportPath = Path.Combine(_path, "Rpt_Invoice_Mandob.pdf");
            PdfExportOptions pdfOptions = report.ExportOptions.Pdf;
            pdfOptions.Compressed = true;
            pdfOptions.ImageQuality = PdfJpegImageQuality.Low;
            pdfOptions.NeverEmbeddedFonts = "Tahoma;Courier New";
            pdfOptions.DocumentOptions.Application = "Human Resources Application";
            pdfOptions.DocumentOptions.Author = "اسواق جزيرة الشفاء";
            pdfOptions.DocumentOptions.Subject = "الفاتورة";
            pdfOptions.DocumentOptions.Title = "الفاتورة";
            pdfOptions.ShowPrintDialogOnOpen = true;
            MemoryStream stream = new MemoryStream();
            report.CreateDocument();
            report.ExportToPdf(stream);
            return File(stream.GetBuffer(), "application/pdf");
        }
        [HttpGet]
        public ActionResult loadpdf()
        {
            Response.AddHeader("Content-Disposition", "inline; filename=Master-Agreement.pdf");
            return File("~/Reports/pdf/Rpt_Invoice_Mandob.pdf", "application/pdf");
        }




        //جلب شاشاة دخول للمدير لتأكيد عملية الارجاع----------------------
        [HttpGet]
        public ActionResult AuthorizeManager()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult Omola()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult Omola_Confirm()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult IsAuthorizeManager(Cls_LogIn Cls_LogIn)
        {
            User User = db.Users.Where(a => a.Username == Cls_LogIn.UserName && a.Password == Cls_LogIn.Password).FirstOrDefault();
            if (User != null)
            {

                UserView UserView = db.UserViews.Where(a => a.UserID == User.ID && a.ViewID == viewid).FirstOrDefault();
                if (UserView != null && UserView.Role_Delete == true)
                {

                    return Content("1", "application/json");
                }
                else
                {
                    return Content("2", "application/json");
                }
            }
            {
                return Content("3", "application/json");
            }
        }

        [CustomAuthorize]
        [HttpPost]
        public ActionResult EditDateInvoice(Cls_Invoice_Mandob Cls_Invoice_Mandob)
        {
            Invoice_Mandob Invoice_Mandob = db.Invoice_Mandob.Find(Cls_Invoice_Mandob.ID);
            //*********************************************************************
            System.Globalization.DateTimeFormatInfo HijriDTFI;
            HijriDTFI = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
            HijriDTFI.Calendar = new System.Globalization.HijriCalendar();
            HijriDTFI.ShortDatePattern = "dd/MM/yyyy";

            DateTime Date_Invoice = DateTime.ParseExact(Cls_Invoice_Mandob.Date_Invoice, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string Date_Invoice_Hijri = Date_Invoice.Date.ToString("dd/MM/yyyy", HijriDTFI);
            Invoice_Mandob.Date_Invoice = Date_Invoice;
            Invoice_Mandob.Date_Invoice_Hijri = Date_Invoice_Hijri;
            //*********************************************************************
            int Invoice_Id = Cls_Invoice_Mandob.ID;
            decimal Total_Sadad = db.Invoice_Company_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();
            Invoice_Mandob.Total_Sadad = Total_Sadad;
            db.Entry(Invoice_Mandob).State = EntityState.Modified;
            db.SaveChanges();
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = Invoice_Mandob.ID;
            return Json(Error, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetProducts()
        {
            return PartialView();
        }

      
        [CustomAuthorize]
        [HttpPost]
        public ActionResult SaveOmola(Cls_Invoice_Mandob Cls_Invoice_Mandob)
        {
            Invoice_Mandob Invoice_Mandob = db.Invoice_Mandob.Find(Cls_Invoice_Mandob.ID);
            Invoice_Mandob.Omola_Money = Cls_Invoice_Mandob.Omola_Money_Orignal;
            db.Entry(Invoice_Mandob).State = EntityState.Modified;
            db.SaveChanges();
            //*********************************************************************
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            System.Globalization.DateTimeFormatInfo HijriDTFI;
            HijriDTFI = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
            HijriDTFI.Calendar = new System.Globalization.HijriCalendar();
            HijriDTFI.ShortDatePattern = "dd/MM/yyyy";
            DateTime Date_Invoice = DateTime.ParseExact(Cls_Invoice_Mandob.Date_Invoice, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string Date_Invoice_Hijri = Date_Invoice.Date.ToString("dd/MM/yyyy", HijriDTFI);

            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();
            int Invoice_Number = db.Masrofats.Where(a => a.Is_Deleted == false).Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
            Masrofat Masrofat = new Masrofat
            {
                Invoice_Number = Invoice_Number,
                Invoice_Mandob_ID = Invoice_Mandob.ID,
                Date_Invoice = Date_Invoice,
                Masrofat_Type_Id = 2, // صرف عمولة مبيعات
                Money = Invoice_Mandob.Omola_Money ?? default(decimal),
                Bian = "سداد عمولة مندوب مبيعات لفاتورة مبيعات رقم  " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob.Invoice_Number.ToString()) + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob.Date_Invoice.ToString("yyyy/MM/dd"))
                   + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Mandob.Omola_Money ?? default(decimal)))
                   + " ريال ",
                Userid_In = UserID,
                ComputerName = computerDetails[0],
                ComputerUser = computerDetails[1],
                InDate = Date_Invoice
            };
            db.Masrofats.Add(Masrofat);
            db.SaveChanges();
            //*********************************************************************
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.Omola_Money = Invoice_Mandob.Omola_Money ?? default(decimal);

            return Json(Error, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult PDF_SarfOmola(int id)
        {
            List<Cls_Invoice_Mandob_Sadad> list = new List<Cls_Invoice_Mandob_Sadad>();
            Invoice_Mandob Invoice_Mandob = db.Invoice_Mandob.Find(id);

            Cls_Invoice_Mandob_Sadad Cls_Invoice_Mandob_Sadad = new Cls_Invoice_Mandob_Sadad
            {

                Warehouse_Address = "الحدود الشمالية - محافظة رفحاء - طريق عمر بن الخطاب - حي الروضة",
                Warehouse_Mobile = ConvertToEasternArabicNumerals.ConvertAR("0553390093"),
                Warehouse_Email = "naqirafha@gmail.com",
                ID = ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob.Invoice_Number.ToString()),
                Invoice_Number = ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob.Invoice_Number.ToString()),

                Money = ConvertToEasternArabicNumerals.ConvertAR(((int)Invoice_Mandob.Omola_Money).ToString()),
                Halala = ConvertToEasternArabicNumerals.GetDecimal_only(Invoice_Mandob.Omola_Money ?? default(decimal)),
                Money_Tafkeet = new ToWord(Invoice_Mandob.Omola_Money ?? default(decimal), new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia)).ConvertToArabic(),
                Mandob_Name = Invoice_Mandob.Mandob.Name,
                Date_Invoice = ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob.Date_Invoice.ToString("yyyy/MM/dd")),
                Sadad_Date_Added = ConvertToEasternArabicNumerals.ConvertAR(DateTime.Now.ToString("yyyy/MM/dd")),
                Sadad_Time = ConvertToEasternArabicNumerals.ConvertAR(DateTime.Now.ToString("tt hh:mm"))
            };

            list.Add(Cls_Invoice_Mandob_Sadad);
            // Rpt_Product Rpt_Product = new Rpt_Product();

            //string _path = System.Web.HttpContext.Current.Server.MapPath(@"~/Reports/pdf");
            //Random random = new Random();
            //string tick = DateTime.Now.Ticks.ToString();
            //string reportPath = Path.Combine(_path, "BarCodeReport.pdf");

            Rpt_Invoice_Mandob_SarfOmola report = new Rpt_Invoice_Mandob_SarfOmola();
            report.DataSource = list;

            PdfExportOptions pdfOptions = report.ExportOptions.Pdf;
            pdfOptions.Compressed = true;
            pdfOptions.ImageQuality = PdfJpegImageQuality.Low;
            pdfOptions.NeverEmbeddedFonts = "Tahoma;Courier New";
            pdfOptions.DocumentOptions.Application = "Human Resources Application";
            pdfOptions.DocumentOptions.Author = "مؤسسة الجود لتقنية المعلومات";
            pdfOptions.DocumentOptions.Subject = "تصفير الايرادات اليومية";
            pdfOptions.DocumentOptions.Title = "تصفير الايرادات اليومية";


            using (MemoryStream stream = new MemoryStream())
            {
                report.CreateDocument();
                report.ExportToPdf(stream);
                return File(stream.GetBuffer(), "application/pdf");
            }
        }

        [HttpGet]
        public ActionResult Pdf_Masrofat(int id = 0)
        {
            ConvertToEasternArabicNumerals ConvertToEasternArabicNumerals = new ConvertToEasternArabicNumerals();
            List<Prnt_Masrofat> List = new List<Prnt_Masrofat>();
            Masrofat Masrofat = db.Masrofats.Where(a => a.Invoice_Mandob_ID == id).FirstOrDefault();

            Prnt_Masrofat Prnt_Masrofat = new Prnt_Masrofat();
            Prnt_Masrofat.Warehouse_Address = "الحدود الشمالية - محافظة رفحاء - طريق عمر بن الخطاب - حي الروضة";
            Prnt_Masrofat.Warehouse_Mobile = ConvertToEasternArabicNumerals.ConvertAR("0553390093");
            Prnt_Masrofat.Warehouse_Email = "naqirafha@gmail.com";
            Prnt_Masrofat.ID = ConvertToEasternArabicNumerals.ConvertAR(Masrofat.ID.ToString());
            Prnt_Masrofat.Bian = ConvertToEasternArabicNumerals.ConvertAR(Masrofat.Bian.ToString());
            Prnt_Masrofat.Date_Invoice = ConvertToEasternArabicNumerals.ConvertAR(Masrofat.Date_Invoice.ToString("yyyy/MM/dd"));
            Prnt_Masrofat.Notes = ConvertToEasternArabicNumerals.ConvertAR(Masrofat.Notes);
            Prnt_Masrofat.Money = ConvertToEasternArabicNumerals.ConvertAR(double.Parse(Masrofat.Money.ToString()).ToString());
            Prnt_Masrofat.Invoice_Number = ConvertToEasternArabicNumerals.ConvertAR(Masrofat.Invoice_Number.ToString());
            Prnt_Masrofat.Masrofat_Type_Name = Masrofat.Masrofat_Type.Name;
            List.Add(Prnt_Masrofat);


            Rpt_Masrofat report = new Rpt_Masrofat();
            report.DataSource = List;
            //string _path = System.Web.HttpContext.Current.Server.MapPath(@"~/Reports/pdf");
            //string reportPath = Path.Combine(_path, "Rpt_Invoice_Mandob.pdf");


            PdfExportOptions pdfOptions = report.ExportOptions.Pdf;
            pdfOptions.Compressed = true;
            pdfOptions.ImageQuality = PdfJpegImageQuality.Low;
            pdfOptions.NeverEmbeddedFonts = "Tahoma;Courier New";
            pdfOptions.DocumentOptions.Application = "Human Resources Application";
            pdfOptions.DocumentOptions.Author = "اسواق جزيرة الشفاء";
            pdfOptions.DocumentOptions.Subject = "الفاتورة";
            pdfOptions.DocumentOptions.Title = "الفاتورة";
            pdfOptions.ShowPrintDialogOnOpen = true;


            MemoryStream stream = new MemoryStream();
            report.CreateDocument();
            report.ExportToPdf(stream);

            return File(stream.GetBuffer(), "application/pdf");


        }

        [HttpGet]
        public ActionResult GetCustomer_Type()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult getAllCustomer_Type(string Search = null)
        {
            List<Cls_CustomerType> AllRecords = new List<Cls_CustomerType>();
            AllRecords.Add(new Cls_CustomerType
            {
                Customer_Type=1,
                Customer_Name= "مندوب"
            });
            AllRecords.Add(new Cls_CustomerType
            {
                Customer_Type = 2,
                Customer_Name = "محل"
            });
            AllRecords.Add(new Cls_CustomerType
            {
                Customer_Type = 3,
                Customer_Name = "منزل"
            });
            var list = JsonConvert.SerializeObject(AllRecords, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
            //return Json(AllRecords, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetPayment_Type()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult getAllPayment_Type(string Search = null)
        {
            List<Cls_Payment_Type> AllRecords = new List<Cls_Payment_Type>();
            AllRecords.Add(new Cls_Payment_Type
            {
                Payment_Type = 1,
                Payment_Type_Name = "كاش"
            });
            AllRecords.Add(new Cls_Payment_Type
            {
                Payment_Type = 2,
                Payment_Type_Name = "اجل"
            });
           
            var list = JsonConvert.SerializeObject(AllRecords, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
            //return Json(AllRecords, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}