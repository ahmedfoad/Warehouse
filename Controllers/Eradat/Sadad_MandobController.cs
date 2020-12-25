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
using Warehouse.CustomFilters;
using Microsoft.AspNet.Identity;
using Warehouse.Reports.Eradat;

namespace Warehouse.Controllers.Main
{
    public class Sadad_MandobController : Controller
    {

        ConvertToEasternArabicNumerals ConvertToEasternArabicNumerals = new ConvertToEasternArabicNumerals();
        DB_StoreEntities db = new DB_StoreEntities();
        string[] allFormats = { "yyyy/MM/dd", "yyyy/M/d", "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy", "d/MM/yyyy", "yyyy-MM-dd", "yyyy-M-d", "dd-MM-yyyy", "d-M-yyyy", "dd-M-yyyy", "d-MM-yyyy", "yyyy MM dd", "yyyy M d", "dd MM yyyy", "d M yyyy", "dd M yyyy", "d MM yyyy" };
        ErrorViewModel Error = new ErrorViewModel();
        private int recordsPerPage = 300;
        int viewid = 2474;// حركة بيع
                          // GET: Sadad_Mandob
        [CustomAuthorize]
        [HttpGet]
        public ActionResult SadadMandob(int id = 0)
        {
            return View();
        }


        [HttpGet]
        public ActionResult getAllProducts(int invoice_id)
        {
            List<ClsInvoiceMandob_Product> AllRecords = new List<ClsInvoiceMandob_Product>(); ;
            var Mylist = db.Invoice_Mandob_Product
           .Where(s => s.Invoice_Mandob_Id == invoice_id)
           .OrderBy(s => s.Product.Name)
           .Take(recordsPerPage).ToList();

            foreach (var item in Mylist)
            {
                int Return_Amount = db.Invoice_Mandob_Sadad_Rproduct.Where(a => a.Invoice_Mandob_Sadad.Invoice_Id == invoice_id && a.Return_Invoice_Product_id == item.Product_Id).Select(a => a.Amount).DefaultIfEmpty(0).Sum();
                int Remain_Amount = item.Amount - Return_Amount;
                if (Remain_Amount > 0)
                {
                    ClsInvoiceMandob_Product ClsInvoiceMandob_Product = new ClsInvoiceMandob_Product
                    {
                        ID = item.ID,
                        Product_Id = item.Product_Id,
                        Product_Name = item.Product_Name,
                        Amount = Remain_Amount,
                        Price = item.Price,
                        TotalPrice = item.TotalPrice,
                        //Taxes = (a.Price_Unit * (a.Taxes / 100)),
                        Return_Amount = 0,
                        Remain_Amount = 0,
                        Return_Price = 0
                        //Taxes = a.Taxes,

                    };
                    AllRecords.Add(ClsInvoiceMandob_Product);
                }
            }


            var list = JsonConvert.SerializeObject(AllRecords, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
            //return Json(AllRecords, JsonRequestBehavior.AllowGet);
        }

       

        [HttpGet]
        public ActionResult Create()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult popup_SadadMandob()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult Edit()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult GetProducts()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult Load_SadadMandob(int id)
        {
            Cls_Invoice_Mandob Cls_Invoice_Mandob = new Cls_Invoice_Mandob();
            Invoice_Mandob Invoice_Mandob = db.Invoice_Mandob.Find(id);
            Cls_Invoice_Mandob.ID = Invoice_Mandob.ID;
            Cls_Invoice_Mandob.Mandob_id = Invoice_Mandob.Mandob_id;
            Cls_Invoice_Mandob.Mandob_Name = Invoice_Mandob.Mandob.Name;
            Cls_Invoice_Mandob.Date_Invoice = Invoice_Mandob.Date_Invoice.ToString("yyyy-MM-dd");
            Cls_Invoice_Mandob.Date_Invoice_Hijri = Invoice_Mandob.Date_Invoice_Hijri;
            Cls_Invoice_Mandob.Price = Invoice_Mandob.Price;
            decimal Total_Sadad = db.Invoice_Mandob_Sadad.Where(a => a.Invoice_Id == id && a.Sadad_Type_Id != 4 && a.Invoice_Mandob.Is_Deleted == false).Select(a => a.Money).DefaultIfEmpty(0).Sum();
            decimal Total_Return = db.Invoice_Mandob_Sadad.Where(a => a.Invoice_Id == id && a.Sadad_Type_Id == 4 && a.Invoice_Mandob.Is_Deleted == false).Select(a => a.Money).DefaultIfEmpty(0).Sum();
            //*****************************************
            Total_Sadad  = Total_Sadad + Total_Return;
            //**************************************
            Cls_Invoice_Mandob.Total_Sadad = Total_Sadad;
            Cls_Invoice_Mandob.Total_Return = Total_Return;
            Cls_Invoice_Mandob.Remain = Invoice_Mandob.Price - (Total_Sadad - Total_Return);

            // var Current_Price = Invoice_Mandob.Price;
            var Current_Price = Total_Sadad;
            if (db.Invoice_Mandob_Sadad.Where(a => a.Invoice_Id == id && a.Invoice_Mandob.Is_Deleted == false).Any() == true)
            {
                Cls_Invoice_Mandob.Sadad_Count = db.Invoice_Mandob_Sadad.Where(a => a.Invoice_Id == id && a.Invoice_Mandob.Is_Deleted == false).Count();
                Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad = new List<ClsInvoice_Mandob_Sadad>();
                foreach (var item in Invoice_Mandob.Invoice_Mandob_Sadad)
                {
                    Current_Price = Current_Price - item.Money;
                    Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad.Add(
                        new ClsInvoice_Mandob_Sadad
                        {
                            ID = item.ID,
                            Invoice_Id = item.Invoice_Id,
                            Sadad_Type_Id = item.Sadad_Type_Id,
                            Date_Added = item.Date_Added,
                            Money = item.Money,
                            Remain = Current_Price,
                        }
                        );
                }
            }
            else
            {
                Cls_Invoice_Mandob.Sadad_Count = 0;
            }

            //Cls_Invoice_Mandob.ClsInvoiceMandob_Product = new List<ClsInvoiceMandob_Product>();

            var list = JsonConvert.SerializeObject(Cls_Invoice_Mandob, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });

            return Content(list, "application/json");
        }
        [CustomAuthorize]
        [HttpPost]
        public ActionResult Save_Sadad(ClsInvoice_Mandob_Sadad ClsInvoice_Mandob_Sadad)
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
                Sadad_Type_Id = ClsInvoice_Mandob_Sadad.Sadad_Type_Id,
                Date_Added = Date_Invoice,
                Money = ClsInvoice_Mandob_Sadad.Money,
                //Remain = ClsInvoice_Mandob_Sadad.Remain,

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
            int Invoice_Number = db.Eradats.Where(a => a.Is_Deleted == false).Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
            Eradat Eradat = new Eradat
            {
                Invoice_Number = Invoice_Number,
                Invoice_Mandob_Sadad_ID = Sadad.ID,
                Date_Invoice = Date_Invoice,
                Eradat_Type_Id = 1, // سداد فاتورة مبيعات
                Money = Sadad.Money,
                Bian = "سداد فاتورة مبيعات رقم " + ConvertToEasternArabicNumerals.ConvertAR(Sadad.Invoice_Mandob.Invoice_Number.ToString()) + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob.Date_Invoice.ToString("yyyy/MM/dd"))
                   + " قيمة الفاتورة " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Mandob.Price))
                   + " رقم سند القبض " + ConvertToEasternArabicNumerals.ConvertAR(Sadad.ID.ToString())
                   + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Sadad.Money))
                   + " ريال إجمالي السداد " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Total_Sadad))
                   + " ريال المبلغ المتبقي " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Remain))
                   + " ريال ",
                Userid_In = UserID,
                ComputerName = computerDetails[0],
                ComputerUser = computerDetails[1],
                InDate = Date_Invoice

            };
            db.Eradats.Add(Eradat);
            db.SaveChanges();
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.Invoice_Product_ID = Sadad.ID;
            Error.Date_Added = Sadad.Date_Added;
            Error.Sadad_Type_Id = Sadad.Sadad_Type_Id;
            // return Json(Error, JsonRequestBehavior.AllowGet);

            var list = JsonConvert.SerializeObject(Error, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });

            return Content(list, "application/json");

        }

        [CustomAuthorize]
        [HttpPost]
        public ActionResult Edit_Sadad(ClsInvoice_Mandob_Sadad ClsInvoice_Mandob_Sadad)
        {
            DateTime Date_Invoice = DateTime.Now;
            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            //Add New
            int id = ClsInvoice_Mandob_Sadad.ID;
            Invoice_Mandob_Sadad Invoice_Mandob_Sadad = db.Invoice_Mandob_Sadad.Find(id);
            Invoice_Mandob_Sadad.Sadad_Type_Id = ClsInvoice_Mandob_Sadad.Sadad_Type_Id;
            Invoice_Mandob_Sadad.Date_Added = Date_Invoice;
            Invoice_Mandob_Sadad.Money = ClsInvoice_Mandob_Sadad.Money;
            db.Entry(Invoice_Mandob_Sadad).State = EntityState.Modified;
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
            Eradat Eradat = db.Eradats.Where(a => a.Invoice_Mandob_Sadad_ID == id).FirstOrDefault();

            if (Eradat != null)
            {


                Eradat.Invoice_Mandob_Sadad_ID = Invoice_Mandob_Sadad.ID;
                Eradat.Date_Invoice = Date_Invoice;
                Eradat.Eradat_Type_Id = 1; // سداد فاتورة مبيعات
                Eradat.Money = Invoice_Mandob_Sadad.Money;
                Eradat.Bian = "سداد فاتورة مبيعات رقم " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob_Sadad.Invoice_Mandob.Invoice_Number.ToString()) + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob.Date_Invoice.ToString("yyyy/MM/dd"))
                  + " قيمة الفاتورة " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Mandob.Price))
                  + " رقم سند القبض " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob_Sadad.ID.ToString())
                  + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Mandob_Sadad.Money))
                  + " ريال إجمالي السداد " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Total_Sadad))
                  + " ريال المبلغ المتبقي " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Remain))
                  + " ريال ";
                Eradat.Userid_In = UserID;
                Eradat.ComputerName = computerDetails[0];
                Eradat.ComputerUser = computerDetails[1];
                Eradat.InDate = Date_Invoice;
                db.Entry(Eradat).State = EntityState.Modified;
                db.SaveChanges();

            }
            else
            {
                int Invoice_Number = db.Eradats.Where(a => a.Is_Deleted == false).Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
                Eradat = new Eradat
                {
                    Invoice_Number = Invoice_Number,
                    Invoice_Mandob_Sadad_ID = Invoice_Mandob_Sadad.ID,
                    Date_Invoice = Date_Invoice,
                    Eradat_Type_Id = 1, // سداد فاتورة مبيعات
                    Money = Invoice_Mandob_Sadad.Money,
                    Bian = "سداد فاتورة مبيعات رقم " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob_Sadad.Invoice_Mandob.Invoice_Number.ToString()) + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob.Date_Invoice.ToString("yyyy/MM/dd"))
                   + " قيمة الفاتورة " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Mandob.Price))
                   + " رقم سند القبض " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob_Sadad.ID.ToString())
                   + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Mandob_Sadad.Money))
                   + " ريال إجمالي السداد " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Total_Sadad))
                   + " ريال المبلغ المتبقي " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Remain))
                   + " ريال ",
                    Userid_In = UserID,
                    ComputerName = computerDetails[0],
                    ComputerUser = computerDetails[1],
                    InDate = Date_Invoice

                };
                db.Eradats.Add(Eradat);
                db.SaveChanges();
            }
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.Invoice_Product_ID = Invoice_Mandob_Sadad.ID;
            Error.Date_Added = Invoice_Mandob_Sadad.Date_Added;
            Error.Sadad_Type_Id = Invoice_Mandob_Sadad.Sadad_Type_Id;
            // return Json(Error, JsonRequestBehavior.AllowGet);

            var list = JsonConvert.SerializeObject(Error, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });

            return Content(list, "application/json");

        }

        [HttpPost]
        public ActionResult Save_ReturnPorducts(ClsInvoice_Mandob_Sadad New_Sadad, List<ClsInvoiceMandob_Product> ProductList)
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
                Invoice_Id = New_Sadad.Invoice_Id,
                Sadad_Type_Id = New_Sadad.Sadad_Type_Id,
                Date_Added = Date_Invoice,
                Money = (New_Sadad.Money * (-1)) ,
                //Remain = ClsInvoice_Mandob_Sadad.Remain,

                User_ID = UserID,
                ComputerName = computerDetails[0],
                ComputerUser = computerDetails[1],
                InDate = Date_Invoice
            };
            db.Invoice_Mandob_Sadad.Add(Sadad);
            db.SaveChanges();
            foreach (var item in ProductList)
            {
                Invoice_Mandob_Sadad_Rproduct Invoice_Mandob_Sadad_Rproduct = new Invoice_Mandob_Sadad_Rproduct
                {
                    Invoice_Mandob_Sadad_id = Sadad.ID,

                    Return_Invoice_Product_id = item.Product_Id,
                    Amount = item.Return_Amount,
                    Price = item.Return_Price,

                    InDate = Date_Invoice
                };
                db.Invoice_Mandob_Sadad_Rproduct.Add(Invoice_Mandob_Sadad_Rproduct);
                db.SaveChanges();
            }
            int Invoice_Id = New_Sadad.Invoice_Id;
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
                Money = New_Sadad.Money,
                Bian = "ارجاع الاصناف لفاتورة مبيعات رقم  " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Mandob.Invoice_Number.ToString())) + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob.Date_Invoice.ToString("yyyy/MM/dd"))
                   + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", New_Sadad.Money))
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
            Error.Invoice_Product_ID = Sadad.ID;
            Error.Date_Added = Sadad.Date_Added;
            Error.Sadad_Type_Id = Sadad.Sadad_Type_Id;
            // return Json(Error, JsonRequestBehavior.AllowGet);

            var list = JsonConvert.SerializeObject(Error, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });

            return Content(list, "application/json");

        }

        [HttpGet]
        public ActionResult PDF_Sadad(int id)
        {
            List<Cls_Invoice_Mandob_Sadad> list = new List<Cls_Invoice_Mandob_Sadad>();
            Invoice_Mandob_Sadad Invoice_Mandob_Sadad = db.Invoice_Mandob_Sadad.Find(id);

            Cls_Invoice_Mandob_Sadad Cls_Invoice_Mandob_Sadad = new Cls_Invoice_Mandob_Sadad
            {

                Warehouse_Address = "الحدود الشمالية - محافظة رفحاء - طريق عمر بن الخطاب - حي الروضة",
                Warehouse_Mobile = ConvertToEasternArabicNumerals.ConvertAR("0553390093"),
                Warehouse_Email = "naqirafha@gmail.com",
                ID = ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob_Sadad.ID.ToString()),
                Invoice_Number = ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob_Sadad.Invoice_Mandob.Invoice_Number.ToString()),

                Money = ConvertToEasternArabicNumerals.ConvertAR(((int)Invoice_Mandob_Sadad.Money).ToString()),
                Halala = ConvertToEasternArabicNumerals.GetDecimal_only(Invoice_Mandob_Sadad.Money),
                Money_Tafkeet = new ToWord(Invoice_Mandob_Sadad.Money, new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia)).ConvertToArabic(),
                Mandob_Name = Invoice_Mandob_Sadad.Invoice_Mandob.Mandob.Name,


                Date_Invoice = ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob_Sadad.Invoice_Mandob.Date_Invoice.ToString("yyyy/MM/dd")),
                Sadad_Date_Added = ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob_Sadad.Date_Added.ToString("yyyy/MM/dd")),
                Sadad_Time = ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob_Sadad.Date_Added.ToString("tt hh:mm"))
            };

            list.Add(Cls_Invoice_Mandob_Sadad);
            // Rpt_Product Rpt_Product = new Rpt_Product();

            //string _path = System.Web.HttpContext.Current.Server.MapPath(@"~/Reports/pdf");
            //Random random = new Random();
            //string tick = DateTime.Now.Ticks.ToString();
            //string reportPath = Path.Combine(_path, "BarCodeReport.pdf");

            Rpt_Invoice_Mandob_Kabd report = new Rpt_Invoice_Mandob_Kabd();
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
        public ActionResult Pdf_Eradat(int id = 0)
        {
            ConvertToEasternArabicNumerals ConvertToEasternArabicNumerals = new ConvertToEasternArabicNumerals();
            List<Prnt_Eradat> List = new List<Prnt_Eradat>();
            Eradat Eradat = db.Eradats.Where(a => a.Invoice_Mandob_Sadad_ID == id).FirstOrDefault();

            Prnt_Eradat Prnt_Eradat = new Prnt_Eradat();
            Prnt_Eradat.Warehouse_Address = "الحدود الشمالية - محافظة رفحاء - طريق عمر بن الخطاب - حي الروضة";
            Prnt_Eradat.Warehouse_Mobile = ConvertToEasternArabicNumerals.ConvertAR("0553390093");
            Prnt_Eradat.Warehouse_Email = "naqirafha@gmail.com";
            Prnt_Eradat.ID = ConvertToEasternArabicNumerals.ConvertAR(Eradat.ID.ToString());
            Prnt_Eradat.Bian = ConvertToEasternArabicNumerals.ConvertAR(Eradat.Bian.ToString());
            Prnt_Eradat.Date_Invoice = ConvertToEasternArabicNumerals.ConvertAR(Eradat.Date_Invoice.ToString("yyyy/MM/dd"));
            Prnt_Eradat.Notes = ConvertToEasternArabicNumerals.ConvertAR(Eradat.Notes);
            Prnt_Eradat.Money = ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Eradat.Money));
            Prnt_Eradat.Invoice_Number = ConvertToEasternArabicNumerals.ConvertAR(Eradat.Invoice_Number.ToString());
            Prnt_Eradat.Eradat_Type_Name = Eradat.Eradat_Type.Name;
            List.Add(Prnt_Eradat);


            Rpt_Eradat report = new Rpt_Eradat();
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


        #region Sadad Mandobs
        [CustomAuthorize(Roles = "سداد فاتورة المبيعات&save$1")]
        // GET: Search
        public ActionResult Sadad()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Sadad(Srch_Sadad_MandobsList RModel)
        {
            //var roundedD = Math.Round(2.5, 0); // Output: 2
            //var roundedE = Math.Round(2.5, 0, MidpointRounding.AwayFromZero); // Output: 3

            var AllRecord = db.Invoice_Mandob.Where(x => x.Price != x.Total_Sadad).AsQueryable();
            if (RModel.Mandob_id != 0)
            {
                AllRecord = AllRecord.Where(x => x.Mandob_id == RModel.Mandob_id);
            }
            if (RModel.Sadad_Status == 1)//لم يتم السداد
            {
                AllRecord = AllRecord.Where(x => x.Price > x.Total_Sadad);

            }
            if (RModel.Sadad_Status == 2)//تم السداد كله
            {
                AllRecord = AllRecord.Where(x => x.Price == x.Total_Sadad);
            }
            List<Invoice_Mandob> Invoice_Mandobs = AllRecord.ToList();

            List<Mandob> Mandobs = db.Mandobs.ToList();
            List<ClsSadad_MandobsList> RecordList = new List<ClsSadad_MandobsList>();
            foreach (var item in Mandobs)
            {
                decimal Price = Invoice_Mandobs.Where(a => a.Mandob_id == item.ID && a.Is_Deleted == false).Select(a => a.Price).DefaultIfEmpty(0).Sum();
                decimal Total_Sadad = Invoice_Mandobs.Where(a => a.Mandob_id == item.ID && a.Is_Deleted == false).Select(a => a.Total_Sadad).DefaultIfEmpty(0).Sum();
                decimal Remain = Price - Total_Sadad;

                if (RModel.Sadad_Status == 1)//لم يتم السداد
                {
                    if (Remain <= 0)
                    {
                        continue;   // Skip the remainder of this iteration.
                    }
                }
                decimal _price = Math.Round(Price, 0, MidpointRounding.AwayFromZero);
                decimal _Total_Sadad = Math.Round(Total_Sadad, 0, MidpointRounding.AwayFromZero);
                if (_price == _Total_Sadad)
                {
                    continue;   // Skip the remainder of this iteration.
                }
                ClsSadad_MandobsList ClsSadad_MandobsList = new ClsSadad_MandobsList
                {
                    Mandob_id = item.ID,
                    Mandob_Name = item.Name,
                    Price = Price,
                    Total_Sadad = Total_Sadad,
                    Remain = Remain
                };

                RecordList.Add(ClsSadad_MandobsList);
            }
            var list = JsonConvert.SerializeObject(RecordList,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });
            return Content(list, "application/json");
        }
        #endregion


        // GET: Search
        public ActionResult Sadad_Confirm()
        {
            return PartialView();
        }

        [CustomAuthorize]
        [HttpPost]
        public ActionResult SaveSadadList(Cls_Mandob_SadadList Cls_Mandob_SadadList)
        {
            List<Invoice_Mandob> Invoice_MandobList = db.Invoice_Mandob.Where(x => x.Mandob_id == Cls_Mandob_SadadList.Mandob_id && x.Price > x.Total_Sadad).ToList();
            DateTime Date_Invoice = DateTime.Now;
            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());

            decimal Remain_Money = Cls_Mandob_SadadList.Money;
            decimal Current_Money = 0;
            string txt_invoice_Number = " رقم ";
            string txt_Sadad_id = " رقم ";
            int Sadad_id = 0;
            decimal Invoice_Mandob_Price = 0;
            decimal Total_Total_Sadad = 0, Total_Remain = 0;

            foreach (var item in Invoice_MandobList)
            {
                if (Remain_Money == 0)
                {
                    //return;
                    continue;   // Skip the remainder of this iteration.
                }
                decimal _price = Math.Round(item.Price, 0, MidpointRounding.AwayFromZero);
                decimal _Total_Sadad = Math.Round(item.Total_Sadad, 0, MidpointRounding.AwayFromZero);
                if (_price == _Total_Sadad)
                {
                    continue;   // Skip the remainder of this iteration.
                }
                Current_Money = item.Price - item.Total_Sadad;
                if (Current_Money > Remain_Money)
                {
                    Current_Money = Remain_Money;
                    Remain_Money = 0;
                }
                else if (Current_Money <= Remain_Money)
                {
                    Remain_Money = Remain_Money - Current_Money;
                }
                Invoice_Mandob_Price = Invoice_Mandob_Price + item.Price;
                //Add New
                Sadad_id = db.Invoice_Mandob_Sadad.Select(a => a.ID).DefaultIfEmpty(0).Max() + 1;
                Invoice_Mandob_Sadad Sadad = new Invoice_Mandob_Sadad
                {
                    ID = Sadad_id,
                    Invoice_Id = item.ID,
                    Sadad_Type_Id = Cls_Mandob_SadadList.Sadad_Type_Id,
                    Date_Added = Date_Invoice,
                    Money = Current_Money,

                    User_ID = UserID,
                    ComputerName = computerDetails[0],
                    ComputerUser = computerDetails[1],
                    InDate = Date_Invoice
                };
                db.Invoice_Mandob_Sadad.Add(Sadad);
                db.SaveChanges();
                int Invoice_Id = item.ID;
                Invoice_Mandob Invoice_Mandob = db.Invoice_Mandob.Find(Invoice_Id);
                decimal Total_Sadad = db.Invoice_Mandob_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();
                decimal Remain = Invoice_Mandob.Price - Total_Sadad;
                Total_Total_Sadad = Total_Total_Sadad + Total_Sadad;
                Total_Remain = Total_Remain + Remain;
                //*********************************************************************
                Invoice_Mandob.Total_Sadad = Total_Sadad;
                db.Entry(Invoice_Mandob).State = EntityState.Modified;
                db.SaveChanges();
                //*********************************************************************
                if (Invoice_MandobList.IndexOf(item) > 0)
                {
                    txt_invoice_Number = txt_invoice_Number + " و رقم ";
                }
                txt_invoice_Number = txt_invoice_Number + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob.Invoice_Number.ToString()) + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob.Date_Invoice.ToString("yyyy/MM/dd"));
                if (Invoice_MandobList.IndexOf(item) > 0)
                {
                    txt_Sadad_id = txt_Sadad_id + " و رقم ";
                }
                txt_Sadad_id = txt_Sadad_id + ConvertToEasternArabicNumerals.ConvertAR(Sadad.ID.ToString());
            }
            if (Sadad_id != 0)
            {
                int Invoice_Number = db.Eradats.Where(a => a.Is_Deleted == false).Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
                Eradat Eradat = new Eradat
                {
                    Invoice_Number = Invoice_Number,
                    Invoice_Mandob_Sadad_ID = Sadad_id,//هو اخر رقم سداد لاخر فاتورة ، المهم انه سداد فاتورة من فواتير المبيعات
                    Date_Invoice = Date_Invoice,
                    Eradat_Type_Id = 1, // سداد فاتورة مبيعات
                    Money = Cls_Mandob_SadadList.Money,
                    Bian = "سداد فاتورة مبيعات  " + txt_invoice_Number
                       + " قيمة الفواتير " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Mandob_Price))
                       + " ريال رقم سند القبض " + txt_Sadad_id
                       + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Cls_Mandob_SadadList.Money))
                       + " ريال إجمالي السداد " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Total_Total_Sadad))
                       + " ريال المبلغ المتبقي " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Total_Remain))
                       + " ريال ",
                    Userid_In = UserID,
                    ComputerName = computerDetails[0],
                    ComputerUser = computerDetails[1],
                    InDate = Date_Invoice

                };
                db.Eradats.Add(Eradat);
                db.SaveChanges();
            }
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.Sadad_ID = Sadad_id;

            // return Json(Error, JsonRequestBehavior.AllowGet);
            var list = JsonConvert.SerializeObject(Error, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
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