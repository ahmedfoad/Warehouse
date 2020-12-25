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
    public class Sadad_CompanyController : Controller
    {

        ConvertToEasternArabicNumerals ConvertToEasternArabicNumerals = new ConvertToEasternArabicNumerals();
        DB_StoreEntities db = new DB_StoreEntities();
        string[] allFormats = { "yyyy/MM/dd", "yyyy/M/d", "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy", "d/MM/yyyy", "yyyy-MM-dd", "yyyy-M-d", "dd-MM-yyyy", "d-M-yyyy", "dd-M-yyyy", "d-MM-yyyy", "yyyy MM dd", "yyyy M d", "dd MM yyyy", "d M yyyy", "dd M yyyy", "d MM yyyy" };
        ErrorViewModel Error = new ErrorViewModel();
        private int recordsPerPage = 300;
        int viewid = 2474;// حركة بيع
                          // GET: Sadad_Company
        [CustomAuthorize]
        [HttpGet]
        public ActionResult SadadCompany(int id = 0)
        {
            return View();
        }


        [HttpGet]
        public ActionResult getAllProducts(int invoice_id, int? page = null, string Search = null, string Src_Barcode = null)
        {
            int? skipRecords = (page != null ? page.Value : 0) * recordsPerPage;
            List<ClsInvoiceCompany_Product> AllRecords = null;
            if (string.IsNullOrEmpty(Src_Barcode) == false)
            {
                var a = db.Invoice_Company_Product.Where(s => s.Invoice_Company_Id == invoice_id && (s.Product.Barcode == Src_Barcode)).FirstOrDefault();
                if (a != null)
                {
                    AllRecords = new List<ClsInvoiceCompany_Product>();
                    ClsInvoiceCompany_Product ClsInvoiceCompany_Product = new ClsInvoiceCompany_Product
                    {
                        ID = a.ID,
                        Product_Name = a.Product_Name,
                        Amount = a.Amount,
                        Price = a.Price,
                        //Taxes = (a.Price_Unit * (a.Taxes / 100)),
                        Taxes = a.Taxes,
                        TotalPrice = a.TotalPrice,
                    };
                    AllRecords.Add(ClsInvoiceCompany_Product);
                }
                else
                {
                    decimal _barcode = decimal.Parse(Src_Barcode);
                    a = db.Invoice_Company_Product.Where(s => s.Invoice_Company_Id == invoice_id && (s.Product.Barcode == _barcode.ToString())).FirstOrDefault();
                    if (a != null)
                    {
                        AllRecords = new List<ClsInvoiceCompany_Product>();
                        ClsInvoiceCompany_Product ClsInvoiceCompany_Product = new ClsInvoiceCompany_Product
                        {
                            ID = a.ID,
                            Product_Name = a.Product_Name,
                            Amount = a.Amount,
                            Price = a.Price,
                            //Taxes = (a.Price_Unit * (a.Taxes / 100)),
                            Taxes = a.Taxes,
                            TotalPrice = a.TotalPrice,
                        };
                        AllRecords.Add(ClsInvoiceCompany_Product);
                        CheckUpdateBarcode(a.Product, Src_Barcode);
                    }
                }



            }
            else
            {
                AllRecords = new List<ClsInvoiceCompany_Product>();
                var Mylist = db.Invoice_Company_Product
               .Where(s => s.Invoice_Company_Id == invoice_id && (string.IsNullOrEmpty(Search) ? true : s.Product.Name.Contains(Search)))
               .OrderBy(s => s.Product.Name).Skip(skipRecords != null ? skipRecords.Value : 0)
               .Take(recordsPerPage).ToList();
                foreach (var a in Mylist)
                {
                    ClsInvoiceCompany_Product ClsInvoiceCompany_Product = new ClsInvoiceCompany_Product
                    {
                        ID = a.ID,
                        Product_Name = a.Product_Name,
                        Amount = a.Amount,
                        Price = a.Price,
                        //Taxes = (a.Price_Unit * (a.Taxes / 100)),
                        Taxes = a.Taxes,
                        TotalPrice = a.TotalPrice,
                    };
                    AllRecords.Add(ClsInvoiceCompany_Product);
                }
            }

            var list = JsonConvert.SerializeObject(AllRecords, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
            //return Json(AllRecords, JsonRequestBehavior.AllowGet);
        }

        private void CheckUpdateBarcode(Product product, string Barcode)
        {
            decimal test = decimal.Parse(Barcode);
            string _barcode = test.ToString();


            if (product.Barcode == Barcode)
            {
                return;
            }
            else
            {
                if (product.Barcode == _barcode)
                {
                    product.Barcode = Barcode;
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                }


            }
        }

        [HttpGet]
        public ActionResult Create()
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
        public ActionResult Load_SadadCompany(int id)
        {
            Cls_Invoice_Company Cls_Invoice_Company = new Cls_Invoice_Company();
            Invoice_Company Invoice_Company = db.Invoice_Company.Find(id);
            Cls_Invoice_Company.ID = Invoice_Company.ID;
            Cls_Invoice_Company.Company_id = Invoice_Company.Company_id;
            Cls_Invoice_Company.Company_Name = Invoice_Company.Company.Name;
            Cls_Invoice_Company.Date_Invoice = Invoice_Company.Date_Invoice.ToString("yyyy-MM-dd");
            Cls_Invoice_Company.Date_Invoice_Hijri = Invoice_Company.Date_Invoice_Hijri;
            Cls_Invoice_Company.Price = Invoice_Company.Price;
            decimal Total_Sadad = db.Invoice_Company_Sadad.Where(a => a.Invoice_Id == id && a.Invoice_Company.Is_Deleted == false && a.Invoice_Company.IS_Chekced == false).Select(a => a.Money).DefaultIfEmpty(0).Sum();
            Cls_Invoice_Company.Total_Sadad = Total_Sadad;
            Cls_Invoice_Company.Remain = Invoice_Company.Price - Total_Sadad;

            var Current_Price = Invoice_Company.Price;
            if (db.Invoice_Company_Sadad.Where(a => a.Invoice_Id == id && a.Invoice_Company.Is_Deleted == false && a.Invoice_Company.IS_Chekced == false).Any() == true)
            {
                Cls_Invoice_Company.Sadad_Count = db.Invoice_Company_Sadad.Where(a => a.Invoice_Id == id && a.Invoice_Company.Is_Deleted == false && a.Invoice_Company.IS_Chekced == false).Count();
                Cls_Invoice_Company.ClsInvoice_Company_Sadad = new List<ClsInvoice_Company_Sadad>();
                foreach (var item in Invoice_Company.Invoice_Company_Sadad)
                {
                    Current_Price = Current_Price - item.Money;
                    Cls_Invoice_Company.ClsInvoice_Company_Sadad.Add(
                        new ClsInvoice_Company_Sadad
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
                Cls_Invoice_Company.Sadad_Count = 0;
            }

            //Cls_Invoice_Company.ClsInvoiceCompany_Product = new List<ClsInvoiceCompany_Product>();

            var list = JsonConvert.SerializeObject(Cls_Invoice_Company, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });

            return Content(list, "application/json");
        }
        [CustomAuthorize]
        [HttpPost]
        public ActionResult Save_Sadad(ClsInvoice_Company_Sadad ClsInvoice_Company_Sadad)
        {
            DateTime Date_Invoice = DateTime.Now;
            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            //Add New
            int _id = db.Invoice_Company_Sadad.Select(a => a.ID).DefaultIfEmpty(0).Max() + 1;
            Invoice_Company_Sadad Sadad = new Invoice_Company_Sadad
            {
                ID= _id,
                Invoice_Id = ClsInvoice_Company_Sadad.Invoice_Id,
                Sadad_Type_Id = ClsInvoice_Company_Sadad.Sadad_Type_Id,
                Date_Added = Date_Invoice,
                Money = ClsInvoice_Company_Sadad.Money,
                //Remain = ClsInvoice_Company_Sadad.Remain,

                User_ID = UserID,
                ComputerName = computerDetails[0],
                ComputerUser = computerDetails[1],
                InDate = Date_Invoice
            };
            db.Invoice_Company_Sadad.Add(Sadad);
            db.SaveChanges();
            int Invoice_Id = ClsInvoice_Company_Sadad.Invoice_Id;
            Invoice_Company Invoice_Company = db.Invoice_Company.Find(Invoice_Id);
            decimal Total_Sadad = db.Invoice_Company_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();
            decimal Remain = Invoice_Company.Price - Total_Sadad;
            //*********************************************************************
            Invoice_Company.Total_Sadad = Total_Sadad;
            db.Entry(Invoice_Company).State = EntityState.Modified;
            db.SaveChanges();
            //*********************************************************************
            int Invoice_Number = db.Masrofats.Where(a => a.Is_Deleted == false).Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
            Masrofat Masrofat = new Masrofat
            {
                Invoice_Number = Invoice_Number,
                Invoice_Company_Sadad_ID = Sadad.ID,
                Date_Invoice = Date_Invoice,
                Masrofat_Type_Id = 1, // سداد فاتورة مشترات
                Money = Sadad.Money,
                Bian = "سداد فاتورة مشتريات رقم " + ConvertToEasternArabicNumerals.ConvertAR(Sadad.Invoice_Company.Invoice_Number.ToString()) + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Company.Date_Invoice.ToString("yyyy/MM/dd"))
                   + " قيمة الفاتورة " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Company.Price))
                   + " رقم سند الصرف " + ConvertToEasternArabicNumerals.ConvertAR(Sadad.ID.ToString())
                   + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Sadad.Money))
                   + " ريال إجمالي السداد " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Total_Sadad))
                   + " ريال المبلغ المتبقي " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Remain))
                   + " ريال ",
                Userid_In = UserID,
                ComputerName = computerDetails[0],
                ComputerUser = computerDetails[1],
                InDate = Date_Invoice

            };
            db.Masrofats.Add(Masrofat);
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
        public ActionResult Edit_Sadad(ClsInvoice_Company_Sadad ClsInvoice_Company_Sadad)
        {
            DateTime Date_Invoice = DateTime.Now;
            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            //Add New
            int id = ClsInvoice_Company_Sadad.ID;
            Invoice_Company_Sadad Invoice_Company_Sadad = db.Invoice_Company_Sadad.Find(id);
            Invoice_Company_Sadad.Sadad_Type_Id = ClsInvoice_Company_Sadad.Sadad_Type_Id;
            Invoice_Company_Sadad.Date_Added = Date_Invoice;
            Invoice_Company_Sadad.Money = ClsInvoice_Company_Sadad.Money;
            db.Entry(Invoice_Company_Sadad).State = EntityState.Modified;
            db.SaveChanges();


            int Invoice_Id = ClsInvoice_Company_Sadad.Invoice_Id;
            Invoice_Company Invoice_Company = db.Invoice_Company.Find(Invoice_Id);
            decimal Total_Sadad = db.Invoice_Company_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();
            decimal Remain = Invoice_Company.Price - Total_Sadad;
            //*********************************************************************
            Invoice_Company.Total_Sadad = Total_Sadad;
            db.Entry(Invoice_Company).State = EntityState.Modified;
            db.SaveChanges();
            //*********************************************************************
            Masrofat Masrofat = db.Masrofats.Where(a => a.Invoice_Company_Sadad_ID == id).FirstOrDefault();
            if (Masrofat != null)
            {


                Masrofat.Invoice_Company_Sadad_ID = Invoice_Company_Sadad.ID;
                Masrofat.Date_Invoice = Date_Invoice;
                Masrofat.Masrofat_Type_Id = 1; // سداد فاتورة مشترات
                Masrofat.Money = Invoice_Company_Sadad.Money;
                Masrofat.Bian = "سداد فاتورة مشتريات رقم " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Company_Sadad.Invoice_Company.Invoice_Number.ToString()) + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Company.Date_Invoice.ToString("yyyy/MM/dd"))
                  + " قيمة الفاتورة " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Company.Price))
                  + " رقم سند الصرف " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Company_Sadad.ID.ToString())
                  + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Company_Sadad.Money))
                  + " ريال إجمالي السداد " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Total_Sadad))
                  + " ريال المبلغ المتبقي " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Remain))
                  + " ريال ";
                Masrofat.Userid_In = UserID;
                Masrofat.ComputerName = computerDetails[0];
                Masrofat.ComputerUser = computerDetails[1];
                Masrofat.InDate = Date_Invoice;
                db.Entry(Masrofat).State = EntityState.Modified;
                db.SaveChanges();

            }
            else
            {
                int Invoice_Number = db.Masrofats.Where(a => a.Is_Deleted == false).Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
                Masrofat = new Masrofat
                {
                    Invoice_Number = Invoice_Number,
                    Invoice_Company_Sadad_ID = Invoice_Company_Sadad.ID,
                    Date_Invoice = Date_Invoice,
                    Masrofat_Type_Id = 1, // سداد فاتورة مشترات
                    Money = Invoice_Company_Sadad.Money,
                    Bian = "سداد فاتورة مشتريات رقم " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Company_Sadad.Invoice_Company.Invoice_Number.ToString()) + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Company.Date_Invoice.ToString("yyyy/MM/dd"))
                   + " قيمة الفاتورة " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Company.Price))
                   + " رقم سند الصرف " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Company_Sadad.ID.ToString())
                   + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Company_Sadad.Money))
                   + " ريال إجمالي السداد " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Total_Sadad.ToString()))
                   + " ريال المبلغ المتبقي " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Remain.ToString()))
                   + " ريال ",
                    Userid_In = UserID,
                    ComputerName = computerDetails[0],
                    ComputerUser = computerDetails[1],
                    InDate = Date_Invoice

                };
                db.Masrofats.Add(Masrofat);
                db.SaveChanges();
            }

            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.Invoice_Product_ID = Invoice_Company_Sadad.ID;
            Error.Date_Added = Invoice_Company_Sadad.Date_Added;
            Error.Sadad_Type_Id = Invoice_Company_Sadad.Sadad_Type_Id;
            // return Json(Error, JsonRequestBehavior.AllowGet);

            var list = JsonConvert.SerializeObject(Error, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });

            return Content(list, "application/json");

        }


        [HttpGet]
        public ActionResult PDF_Sadad(int id)
        {
            List<Cls_Invoice_Company_Sadad> list = new List<Cls_Invoice_Company_Sadad>();
            Invoice_Company_Sadad Invoice_Company_Sadad = db.Invoice_Company_Sadad.Find(id);

            Cls_Invoice_Company_Sadad Cls_Invoice_Company_Sadad = new Cls_Invoice_Company_Sadad
            {
                Warehouse_Address = "الحدود الشمالية - محافظة رفحاء - طريق عمر بن الخطاب - حي الروضة",
                Warehouse_Mobile = ConvertToEasternArabicNumerals.ConvertAR("0553390093"),
                Warehouse_Email = "naqirafha@gmail.com",
                ID = ConvertToEasternArabicNumerals.ConvertAR(Invoice_Company_Sadad.ID.ToString()),
                Company_Invoice_ID = ConvertToEasternArabicNumerals.ConvertAR(Invoice_Company_Sadad.Invoice_Company.ID.ToString()),
                Date_Added = ConvertToEasternArabicNumerals.ConvertAR(Invoice_Company_Sadad.Date_Added.ToString("tt hh:mm yyyy/MM/dd")),
                Money = ConvertToEasternArabicNumerals.ConvertAR(((int)Invoice_Company_Sadad.Money).ToString()),
                Halala = ConvertToEasternArabicNumerals.GetDecimal_only(Invoice_Company_Sadad.Money),
                Money_Tafkeet = new ToWord(Invoice_Company_Sadad.Money, new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia)).ConvertToArabic(),
                Company_Name = Invoice_Company_Sadad.Invoice_Company.Company.Name,
                Date_Invoice = ConvertToEasternArabicNumerals.ConvertAR(Invoice_Company_Sadad.Invoice_Company.Date_Invoice.ToString("yyyy/MM/dd")),
                TimeDate_Invoice = ConvertToEasternArabicNumerals.ConvertAR(Invoice_Company_Sadad.Invoice_Company.Date_Invoice.ToString("tt hh:mm")),
            };
            list.Add(Cls_Invoice_Company_Sadad);
            // Rpt_Product Rpt_Product = new Rpt_Product();

            //string _path = System.Web.HttpContext.Current.Server.MapPath(@"~/Reports/pdf");
            //Random random = new Random();
            //string tick = DateTime.Now.Ticks.ToString();
            //string reportPath = Path.Combine(_path, "BarCodeReport.pdf");

            Rpt_Invoice_Company_Sarf report = new Rpt_Invoice_Company_Sarf();
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
            Masrofat Masrofat = db.Masrofats.Where(a=>a.Invoice_Company_Sadad_ID == id).FirstOrDefault();

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
            //string reportPath = Path.Combine(_path, "Rpt_Invoice_Company.pdf");


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
        public ActionResult popup_SadadCompany()
        {
            return PartialView();
        }

        #region Sadad Companies
        [CustomAuthorize(Roles = "سداد فواتير المشتريات&save$1")]
        // GET: Search
        public ActionResult Sadad()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Sadad(Srch_Sadad_CompaniesList RModel)
        {
            //var roundedD = Math.Round(2.5, 0); // Output: 2
            //var roundedE = Math.Round(2.5, 0, MidpointRounding.AwayFromZero); // Output: 3

            var AllRecord = db.Invoice_Company.Where(x => x.Price != x.Total_Sadad).AsQueryable();
            if (RModel.Company_id != 0)
            {
                AllRecord = AllRecord.Where(x => x.Company_id == RModel.Company_id);
            }
            if (RModel.Sadad_Status == 1)//لم يتم السداد
            {
                AllRecord = AllRecord.Where(x => x.Price > x.Total_Sadad);

            }
            if (RModel.Sadad_Status == 2)//تم السداد كله
            {
                AllRecord = AllRecord.Where(x => x.Price == x.Total_Sadad);
            }
            List<Invoice_Company> Invoice_Companys = AllRecord.ToList();

            List<Company> Companies = db.Companies.ToList();
            List<ClsSadad_CompaniesList> RecordList = new List<ClsSadad_CompaniesList>();
            foreach (var item in Companies)
            {
                decimal Price = Invoice_Companys.Where(a => a.Company_id == item.ID && a.Is_Deleted == false).Select(a => a.Price).DefaultIfEmpty(0).Sum();
                decimal Total_Sadad = Invoice_Companys.Where(a => a.Company_id == item.ID && a.Is_Deleted == false).Select(a => a.Total_Sadad).DefaultIfEmpty(0).Sum();
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
                ClsSadad_CompaniesList ClsSadad_CompaniesList = new ClsSadad_CompaniesList
                {
                    Company_id = item.ID,
                    Company_Name = item.Name,
                    Price = Price,
                    Total_Sadad = Total_Sadad,
                    Remain = Remain
                };

                RecordList.Add(ClsSadad_CompaniesList);
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
        public ActionResult SaveSadadList(Cls_Company_SadadList Cls_Company_SadadList)
        {
            List<Invoice_Company> Invoice_CompanyList = db.Invoice_Company.Where(x => x.Company_id == Cls_Company_SadadList.Company_id && x.Price > x.Total_Sadad).ToList();
            DateTime Date_Invoice = DateTime.Now;
            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());

            decimal Remain_Money = Cls_Company_SadadList.Money;
            decimal Current_Money = 0;
            string txt_invoice_Number = " رقم ";
            string txt_Sadad_id = " رقم ";
            int Sadad_id = 0;
            decimal Invoice_Company_Price = 0;
            decimal Total_Total_Sadad = 0, Total_Remain = 0;

            foreach (var item in Invoice_CompanyList)
            {
                if (Remain_Money == 0)
                {
                    break;
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
                Invoice_Company_Price = Invoice_Company_Price + item.Price;
                //Add New
                Sadad_id = db.Invoice_Company_Sadad.Select(a => a.ID).DefaultIfEmpty(0).Max() + 1;
                Invoice_Company_Sadad Sadad = new Invoice_Company_Sadad
                {
                    ID = Sadad_id,
                    Invoice_Id = item.ID,
                    Sadad_Type_Id = Cls_Company_SadadList.Sadad_Type_Id,
                    Date_Added = Date_Invoice,
                    Money = Current_Money,


                    User_ID = UserID,
                    ComputerName = computerDetails[0],
                    ComputerUser = computerDetails[1],
                    InDate = Date_Invoice
                };
                db.Invoice_Company_Sadad.Add(Sadad);
                db.SaveChanges();
                int Invoice_Id = item.ID;
                Invoice_Company Invoice_Company = db.Invoice_Company.Find(Invoice_Id);
                decimal Total_Sadad = db.Invoice_Company_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();
                decimal Remain = Invoice_Company.Price - Total_Sadad;
                Total_Total_Sadad = Total_Total_Sadad + Total_Sadad;
                Total_Remain = Total_Remain + Remain;
                //*********************************************************************
                Invoice_Company.Total_Sadad = Total_Sadad;
                db.Entry(Invoice_Company).State = EntityState.Modified;
                db.SaveChanges();
                //*********************************************************************
                if (Invoice_CompanyList.IndexOf(item) > 0)
                {
                    txt_invoice_Number = txt_invoice_Number + " و رقم ";
                }
                txt_invoice_Number = txt_invoice_Number + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Company.Invoice_Number.ToString()) + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Company.Date_Invoice.ToString("yyyy/MM/dd"));
                if (Invoice_CompanyList.IndexOf(item) > 0)
                {
                    txt_Sadad_id = txt_Sadad_id + " و رقم ";
                }
                txt_Sadad_id = txt_Sadad_id + ConvertToEasternArabicNumerals.ConvertAR(Sadad.ID.ToString());
            }
            if (Sadad_id != 0)
            {
                int Invoice_Number = db.Masrofats.Where(a => a.Is_Deleted == false).Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;

                Masrofat Masrofat = new Masrofat
                {
                    Invoice_Number = Invoice_Number,
                    Invoice_Company_Sadad_ID = Sadad_id,//هو اخر رقم سداد لاخر فاتورة ، المهم انه سداد فاتورة من فواتير المبيعات
                    Date_Invoice = Date_Invoice,
                    Masrofat_Type_Id = 1, // سداد فاتورة مشترات
                    Money = Cls_Company_SadadList.Money,
                    Bian = "سداد فاتورة مشتريات رقم " + txt_invoice_Number
                      + " قيمة الفواتير " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Cls_Company_SadadList.Money))
                      + " ريال رقم سند الصرف " + ConvertToEasternArabicNumerals.ConvertAR(txt_Sadad_id)
                      + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Cls_Company_SadadList.Money))
                      + " ريال إجمالي السداد " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Total_Total_Sadad))
                      + " ريال المبلغ المتبقي " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Total_Remain))
                      + " ريال ",
                    Userid_In = UserID,
                    ComputerName = computerDetails[0],
                    ComputerUser = computerDetails[1],
                    InDate = Date_Invoice

                };
                db.Masrofats.Add(Masrofat);
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