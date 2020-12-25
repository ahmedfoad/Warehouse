using Warehouse.Models;
using Warehouse.Models.Administration;
using Warehouse.Models.Reports;
using Warehouse.Models.Search;
using Warehouse.Reports.Invoices;
using DevExpress.XtraPrinting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using Warehouse.CustomFilters;
using Microsoft.AspNet.Identity;
using Warehouse.GlobalClass;

namespace Warehouse.Controllers.Main
{
    public class Invoice_CompanyController : Controller
    {
        ConvertToEasternArabicNumerals ConvertToEasternArabicNumerals = new ConvertToEasternArabicNumerals();
        DB_StoreEntities db = new DB_StoreEntities();
        string[] allFormats = { "yyyy/MM/dd", "yyyy/M/d", "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy", "d/MM/yyyy", "yyyy-MM-dd", "yyyy-M-d", "dd-MM-yyyy", "d-M-yyyy", "dd-M-yyyy", "d-MM-yyyy", "yyyy MM dd", "yyyy M d", "dd MM yyyy", "d M yyyy", "dd M yyyy", "d MM yyyy" };
        ErrorViewModel Error = new ErrorViewModel();

        private int recordsPerPage = 300;

        //************************************************************************************************************
        //*************************الفاتورة**************************************************************************
        //************************************************************************************************************
        [CustomAuthorize]
        [HttpGet]
        public ActionResult Invoice_Company()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetCompanies()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult GetSearchProducts()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult GetProducts()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult loadInvoice(int id)
        {
            Cls_Invoice_Company Cls_Invoice_Company = new Cls_Invoice_Company();
            Invoice_Company Invoice_Company = db.Invoice_Company.Find(id);
            if (Invoice_Company != null)
            {
                Cls_Invoice_Company.User = new User { NAME = Invoice_Company.User.NAME, ID = Invoice_Company.User.ID };

                Cls_Invoice_Company.ID = Invoice_Company.ID;
                Cls_Invoice_Company.Invoice_Number = Invoice_Company.Invoice_Number;
                Cls_Invoice_Company.Nakl_Cost = Invoice_Company.Nakl_Cost;

                Cls_Invoice_Company.Company_id = Invoice_Company.Company_id;
                Cls_Invoice_Company.Company_Name = Invoice_Company.Company.Name;
                System.Globalization.GregorianCalendar GregorianCalendar = new GregorianCalendar();
                Cls_Invoice_Company.Date_Invoice = Invoice_Company.Date_Invoice.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.GetCultureInfo("en-us", "en"));
                //Cls_Invoice_Company.Date_Invoice = Invoice_Company.Date_Invoice.ToString("yyyy-MM-dd");
                Cls_Invoice_Company.Date_Invoice_Hijri = Invoice_Company.Date_Invoice_Hijri;
                Cls_Invoice_Company.Price = Invoice_Company.Price;
                Cls_Invoice_Company.Total_Sadad = Invoice_Company.Total_Sadad;
                Cls_Invoice_Company.User_ID = Invoice_Company.User_ID;
                Cls_Invoice_Company.ComputerName = Invoice_Company.ComputerName;
                Cls_Invoice_Company.ComputerUser = Invoice_Company.ComputerUser;
                Cls_Invoice_Company.InDate = Invoice_Company.InDate.ToString("yyyy-MM-dd");
                Cls_Invoice_Company.ClsInvoiceCompany_Product = new List<ClsInvoiceCompany_Product>();
                foreach (var item in Invoice_Company.Invoice_Company_Product)
                {
                    ClsInvoiceCompany_Product ClsInvoiceCompany_Product = new ClsInvoiceCompany_Product();


                    ClsInvoiceCompany_Product.ID = item.ID;
                    ClsInvoiceCompany_Product.Invoice_Company_Id = item.Invoice_Company_Id;
                    ClsInvoiceCompany_Product.Product_Id = item.Product_Id;
                    ClsInvoiceCompany_Product.Product_Name = item.Product_Name;
                    ClsInvoiceCompany_Product.Amount = item.Amount;
                    ClsInvoiceCompany_Product.Taxes = Math.Round(((item.Price * (decimal)0.05)), 3, MidpointRounding.AwayFromZero);
                    ClsInvoiceCompany_Product.Price = item.Price;
                    ClsInvoiceCompany_Product.TotalPrice = item.TotalPrice;
                            
                        

                    Cls_Invoice_Company.ClsInvoiceCompany_Product.Add(ClsInvoiceCompany_Product);
                }
            }

            var list = JsonConvert.SerializeObject(Cls_Invoice_Company, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
        }

        //*********************************************************************************
        //*************************الفاتورة**************************************************************************
        //************************************************************************************************************
       
        [CustomAuthorize]
        [HttpPost]
        public ActionResult AddItem(int index, Cls_Invoice_Company Cls_Invoice_Company)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            DateTime Date_Invoice = DateTime.ParseExact(Cls_Invoice_Company.Date_Invoice, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            int Invoice_ID = 0, Invoice_Product_ID = 0, Invoice_Number=0;
            System.Globalization.DateTimeFormatInfo HijriDTFI;
            HijriDTFI = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
            HijriDTFI.Calendar = new System.Globalization.HijriCalendar();
            HijriDTFI.ShortDatePattern = "dd/MM/yyyy";
            string Date_Invoice_Hijri = Date_Invoice.Date.ToString("dd/MM/yyyy", HijriDTFI);

            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();
            //DateTime Date_Expiration = DateTime.ParseExact(Cls_Invoice_Company.ClsInvoiceCompany_Product[0].Date_Expiration, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //string Date_Expiration_Hijri = Date_Expiration.Date.ToString("dd/MM/yyyy", HijriDTFI);

            if (Cls_Invoice_Company.ID == 0)
            {
                //int Invoice_Id = Cls_Invoice_Company.ID;
                //Invoice_Company Invoice_Company = db.Invoice_Company.Find(Invoice_Id);
                //decimal Total_Sadad = db.Invoice_Company_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();

                Invoice_Number = db.Invoice_Company.Where(a => a.Is_Deleted == false).Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
                Invoice_Company _Invoice_Company = new Invoice_Company
                {
                    Invoice_Number= Invoice_Number,
                    Company_id = Cls_Invoice_Company.Company_id,
                    Nakl_Cost = Cls_Invoice_Company.Nakl_Cost,
                    Date_Invoice = Date_Invoice,
                    Date_Invoice_Hijri = Date_Invoice_Hijri,
                    Price = Cls_Invoice_Company.Price,
                    Taxes = Cls_Invoice_Company.Taxes,
                    Total_Sadad = 0,
                    User_ID = UserID,
                    ComputerName = computerDetails[0],
                    ComputerUser = computerDetails[1],
                    InDate = DateTime.Now,

                };
                db.Invoice_Company.Add(_Invoice_Company);
                db.SaveChanges();
                Invoice_ID = _Invoice_Company.ID;
                Invoice_Company_Product Invoice_Company_Product;



                Invoice_Company_Product = new Invoice_Company_Product
                {
                    Invoice_Company_Id = _Invoice_Company.ID,
                    Product_Id = Cls_Invoice_Company.ClsInvoiceCompany_Product[0].Product_Id,
                    Product_Name = Cls_Invoice_Company.ClsInvoiceCompany_Product[0].Product_Name,
                    Amount = Cls_Invoice_Company.ClsInvoiceCompany_Product[0].Amount,
                    Price = Cls_Invoice_Company.ClsInvoiceCompany_Product[0].Price,
                    Taxes = Cls_Invoice_Company.ClsInvoiceCompany_Product[0].Taxes,
                    TotalPrice = Cls_Invoice_Company.ClsInvoiceCompany_Product[0].TotalPrice,
                };
                db.Invoice_Company_Product.Add(Invoice_Company_Product);
                db.SaveChanges();
                Invoice_Product_ID = Invoice_Company_Product.ID;


            }
            else
            {
                Invoice_Company Invoice_Company = db.Invoice_Company.Find(Cls_Invoice_Company.ID);
                Invoice_Number = Invoice_Company.Invoice_Number;
                Invoice_Company.Company_id = Cls_Invoice_Company.Company_id;
                Invoice_Company.Date_Invoice = Date_Invoice;
                Invoice_Company.Date_Invoice_Hijri = Date_Invoice_Hijri;
                Invoice_Company.Price = Cls_Invoice_Company.Price;
                Invoice_Company.Taxes = Cls_Invoice_Company.Taxes;
                Invoice_Company.Nakl_Cost = Cls_Invoice_Company.Nakl_Cost;
                //*********************************************************************
                int Invoice_Id = Cls_Invoice_Company.ID;
                decimal Total_Sadad = db.Invoice_Company_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();
                Invoice_Company.Total_Sadad = Total_Sadad;
                //*********************************************************************
                Invoice_Company.User_ID = UserID;
                Invoice_Company.ComputerName = computerDetails[0];
                Invoice_Company.ComputerUser = computerDetails[1];
                Invoice_Company.InDate = DateTime.Now;

                db.Entry(Invoice_Company).State = EntityState.Modified;
                db.SaveChanges();
                Invoice_ID = Invoice_Company.ID;
                
                Invoice_Company_Product Invoice_Company_Product = new Invoice_Company_Product
                {
                    Invoice_Company_Id = Invoice_Company.ID,
                    Product_Id = Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Id,
                    Product_Name = Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Name,
                    Amount = Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Amount,
                    Price = Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Price,

                    Taxes = Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Taxes,
                    TotalPrice = Cls_Invoice_Company.ClsInvoiceCompany_Product[index].TotalPrice,
                  
                };
                db.Invoice_Company_Product.Add(Invoice_Company_Product);
                db.SaveChanges();

                Invoice_Product_ID = Invoice_Company_Product.ID;

            }

            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = Invoice_ID;
            Error.Invoice_Number = Invoice_Number;
            Error.Invoice_Product_ID = Invoice_Product_ID;
            Error.index = index;
            return Json(Error, JsonRequestBehavior.AllowGet);

        }
    

        [CustomAuthorize]
        [HttpPost]
        public ActionResult EditItem(int index, int Invoice_Product_ID, Cls_Invoice_Company Cls_Invoice_Company)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            System.Globalization.DateTimeFormatInfo HijriDTFI;
            HijriDTFI = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
            HijriDTFI.Calendar = new System.Globalization.HijriCalendar();
            HijriDTFI.ShortDatePattern = "dd/MM/yyyy";

            DateTime Date_Invoice = DateTime.ParseExact(Cls_Invoice_Company.Date_Invoice, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string Date_Invoice_Hijri = Date_Invoice.Date.ToString("dd/MM/yyyy", HijriDTFI);
           

            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();

            ///Edit

            Invoice_Company Invoice_Company = db.Invoice_Company.Find(Cls_Invoice_Company.ID);
            Invoice_Company.Company_id = Cls_Invoice_Company.Company_id;
            Invoice_Company.Date_Invoice = Date_Invoice;
            Invoice_Company.Date_Invoice_Hijri = Date_Invoice_Hijri;
            Invoice_Company.Price = Cls_Invoice_Company.Price;
            Invoice_Company.Taxes = Cls_Invoice_Company.Taxes;
            Invoice_Company.Nakl_Cost = Cls_Invoice_Company.Nakl_Cost;
            //*********************************************************************
            int Invoice_Id = Cls_Invoice_Company.ID;
            decimal Total_Sadad = db.Invoice_Company_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();
            Invoice_Company.Total_Sadad = Total_Sadad;
            //*********************************************************************
            Invoice_Company.User_ID = UserID;
            Invoice_Company.ComputerName = computerDetails[0];
            Invoice_Company.ComputerUser = computerDetails[1];
            Invoice_Company.InDate = DateTime.Now;

            db.Entry(Invoice_Company).State = EntityState.Modified;
            db.SaveChanges();

            Invoice_Company_Product Invoice_Company_Product = db.Invoice_Company_Product.Find(Invoice_Product_ID);

            Invoice_Company_Product.Invoice_Company_Id = Invoice_Company.ID;
            Invoice_Company_Product.Product_Id = Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Id;
            Invoice_Company_Product.Product_Name = Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Name;


            Invoice_Company_Product.Amount = Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Amount;
            //KKKKKKKKKKKKKKKKKKKKKKKKKKKKK
            Invoice_Company_Product.Price = Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Price;

            Invoice_Company_Product.Taxes = Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Taxes;
            Invoice_Company_Product.TotalPrice = Cls_Invoice_Company.ClsInvoiceCompany_Product[index].TotalPrice;
            db.Entry(Invoice_Company_Product).State = EntityState.Modified;
            db.SaveChanges();


            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = Invoice_Company.ID;
            Error.Invoice_Product_ID = Invoice_Product_ID;
            Error.index = index;
            return Json(Error, JsonRequestBehavior.AllowGet);

        }
        [CustomAuthorize]
        [HttpPost]
        public ActionResult EditDateInvoice(Cls_Invoice_Company Cls_Invoice_Company)
        {
            Invoice_Company Invoice_Company = db.Invoice_Company.Find(Cls_Invoice_Company.ID);
            //*********************************************************************
            System.Globalization.DateTimeFormatInfo HijriDTFI;
            HijriDTFI = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
            HijriDTFI.Calendar = new System.Globalization.HijriCalendar();
            HijriDTFI.ShortDatePattern = "dd/MM/yyyy";

            DateTime Date_Invoice = DateTime.ParseExact(Cls_Invoice_Company.Date_Invoice, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string Date_Invoice_Hijri = Date_Invoice.Date.ToString("dd/MM/yyyy", HijriDTFI);
            Invoice_Company.Date_Invoice = Date_Invoice;
            Invoice_Company.Date_Invoice_Hijri = Date_Invoice_Hijri;
            //*********************************************************************
            int Invoice_Id = Cls_Invoice_Company.ID;
            decimal Total_Sadad = db.Invoice_Company_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();
            Invoice_Company.Total_Sadad = Total_Sadad;
            db.Entry(Invoice_Company).State = EntityState.Modified;
            db.SaveChanges();
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = Invoice_Company.ID;
            return Json(Error, JsonRequestBehavior.AllowGet);

        }
        [CustomAuthorize] 
        [HttpPost]
        public ActionResult EditCompanyName(Cls_Invoice_Company Cls_Invoice_Company)
        {
            Invoice_Company Invoice_Company = db.Invoice_Company.Find(Cls_Invoice_Company.ID);
            //*********************************************************************
            Invoice_Company.Company_id = Cls_Invoice_Company.Company_id;
            //*********************************************************************
            int Invoice_Id = Cls_Invoice_Company.ID;
            decimal Total_Sadad = db.Invoice_Company_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();
            Invoice_Company.Total_Sadad = Total_Sadad;
            db.Entry(Invoice_Company).State = EntityState.Modified;
            db.SaveChanges();
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = Invoice_Company.ID;
            return Json(Error, JsonRequestBehavior.AllowGet);

        }

        [CustomAuthorize]
        [HttpPost]
        public ActionResult EditNakl_Cost(Cls_Invoice_Company Cls_Invoice_Company)
        {
            Invoice_Company Invoice_Company = db.Invoice_Company.Find(Cls_Invoice_Company.ID);
            //*********************************************************************
            Invoice_Company.Price = Cls_Invoice_Company.Price;
            Invoice_Company.Taxes = Cls_Invoice_Company.Taxes;
            Invoice_Company.Nakl_Cost = Cls_Invoice_Company.Nakl_Cost;
            //*********************************************************************
            int Invoice_Id = Cls_Invoice_Company.ID;
            decimal Total_Sadad = db.Invoice_Company_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();
            Invoice_Company.Total_Sadad = Total_Sadad;
            db.Entry(Invoice_Company).State = EntityState.Modified;
            db.SaveChanges();
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = Invoice_Company.ID;
            return Json(Error, JsonRequestBehavior.AllowGet);

        }
        [CustomAuthorize]
        [HttpPost]
        public ActionResult DeleteItem(int My_Invoice_Product_id, Cls_Invoice_Company Cls_Invoice_Company)
        {
            Invoice_Company_Product Invoice_Company_Product = db.Invoice_Company_Product.Find(My_Invoice_Product_id);
            db.Invoice_Company_Product.Remove(Invoice_Company_Product);
            db.SaveChanges();

            Invoice_Company Invoice_Company = db.Invoice_Company.Find(Cls_Invoice_Company.ID);
            //*********************************************************************
            Invoice_Company.Price = Cls_Invoice_Company.Price;
            Invoice_Company.Taxes = Cls_Invoice_Company.Taxes;
            Invoice_Company.Nakl_Cost = Cls_Invoice_Company.Nakl_Cost;
            //*********************************************************************
            int Invoice_Id = Cls_Invoice_Company.ID;
            decimal Total_Sadad = db.Invoice_Company_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();
            Invoice_Company.Total_Sadad = Total_Sadad;
            db.Entry(Invoice_Company).State = EntityState.Modified;
            db.SaveChanges();
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            return Json(Error, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult DeleteInvoice(int ID)
        {

            Invoice_Company Invoice_Company = db.Invoice_Company.Find(ID);
            Invoice_Company.Is_Deleted = true;

            db.Entry(Invoice_Company).State = EntityState.Modified;
            db.SaveChanges();

            Error.ErrorName = "تم الحذف بنجاح ... جاري إعادة تحميل الصفحة";
            //Error.Invoice_Product_ID = Invoice_Company_Product.ID;
            //Error.index = index;
            return Json(Error, JsonRequestBehavior.AllowGet);

        }
        //*********************************************************************************
         



        [HttpGet]
        public ActionResult LoadMoneyForSadad(int company_id)
        {
            Cls_Company_Sadad Cls_Company_Sadad = new Cls_Company_Sadad();

            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            User user = db.Users.Find(UserID);
            Cls_Company_Sadad.User_Name = user.NAME;



            decimal Invoice_Price = db.Invoice_Company.Where(a => a.Company_id == company_id && a.Is_Deleted == false && a.IS_Chekced == false).Select(a => a.Price).DefaultIfEmpty(0).Sum();
            decimal SadadMoney = db.Invoice_Company_Sadad.Where(a => a.Invoice_Company.Company_id == company_id && a.Invoice_Company.Is_Deleted == false && a.Invoice_Company.IS_Chekced == false).Select(a => a.Money).DefaultIfEmpty(0).Sum();

            decimal Remain = Invoice_Price - SadadMoney;
            Cls_Company_Sadad.Company_InvoicePrice = Invoice_Price.ToString();
            Cls_Company_Sadad.Company_MoneySadad = SadadMoney.ToString();
            Cls_Company_Sadad.Company_Money_Remain = Remain.ToString();
            var list = JsonConvert.SerializeObject(Cls_Company_Sadad, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");

        }

        //}
        [HttpGet]
        public ActionResult PdfInvoice(int id = 0)
        {

            List<Prnt_InvoiceCompany_Product> List = new List<Prnt_InvoiceCompany_Product>();
            Invoice_Company Invoice_Company = db.Invoice_Company.Find(id);
            List<Invoice_Company_Product> list = db.Invoice_Company_Product.Where(a => a.Invoice_Company_Id == id).ToList();
            string TotalPrice_Tafkeet = new ToWord(Invoice_Company.Price, new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia)).ConvertToArabic();
            foreach (var a in list)
            {
                Prnt_InvoiceCompany_Product Prnt_InvoiceCompany_Product = new Prnt_InvoiceCompany_Product();
                Prnt_InvoiceCompany_Product.Warehouse_Address = "الحدود الشمالية - محافظة رفحاء - طريق عمر بن الخطاب - حي الروضة";
                Prnt_InvoiceCompany_Product.Warehouse_Mobile = ConvertToEasternArabicNumerals.ConvertAR("0553390093");
                Prnt_InvoiceCompany_Product.Warehouse_Email = "naqirafha@gmail.com";
                Prnt_InvoiceCompany_Product.Invoice_Number = ConvertToEasternArabicNumerals.ConvertAR(a.Invoice_Company.Invoice_Number.ToString());
                Prnt_InvoiceCompany_Product.Nakl_Cost= ConvertToEasternArabicNumerals.ConvertAR(a.Invoice_Company.Nakl_Cost.ToString());
                Prnt_InvoiceCompany_Product.Company_Name = ConvertToEasternArabicNumerals.ConvertAR(a.Invoice_Company.Company.Name.ToString());
                Prnt_InvoiceCompany_Product.Date_Invoice = ConvertToEasternArabicNumerals.ConvertAR(a.Invoice_Company.Date_Invoice.ToString("yyyy/MM/dd"));
                Prnt_InvoiceCompany_Product.Time_Invoice = ConvertToEasternArabicNumerals.ConvertAR(a.Invoice_Company.Date_Invoice.ToString("tt hh:mm"));
                Prnt_InvoiceCompany_Product.Price = ConvertToEasternArabicNumerals.ConvertAR(((double)a.Invoice_Company.Price).ToString());
                Prnt_InvoiceCompany_Product.Taxes = ConvertToEasternArabicNumerals.ConvertAR(((double)a.Invoice_Company.Taxes).ToString());

                Prnt_InvoiceCompany_Product.Product_Taxes = ConvertToEasternArabicNumerals.ConvertAR(Math.Round(((a.Price * (decimal)0.05)), 3, MidpointRounding.AwayFromZero).ToString());
                Prnt_InvoiceCompany_Product.Product_Name = ConvertToEasternArabicNumerals.ConvertAR(a.Product_Name);
                Prnt_InvoiceCompany_Product.Product_Amount = ConvertToEasternArabicNumerals.ConvertAR(a.Amount.ToString());
                Prnt_InvoiceCompany_Product.Product_Price = ConvertToEasternArabicNumerals.ConvertAR(a.Price.ToString());
                Prnt_InvoiceCompany_Product.TotalPrice_Tafkeet = TotalPrice_Tafkeet;
                //decimal ProductTotoal_withouttaxes = Math.Round((((decimal)a.Price * a.Amount)), 3, MidpointRounding.AwayFromZero);
                decimal Product_TotalPrice = Math.Round((((decimal)(a.Price + a.Taxes)* (decimal)a.Amount)), 3, MidpointRounding.AwayFromZero);
                Prnt_InvoiceCompany_Product.Product_TotalPrice = ConvertToEasternArabicNumerals.ConvertAR(Product_TotalPrice.ToString());
                //Prnt_InvoiceCompany_Product.Product_TotalPrice = ConvertToEasternArabicNumerals.ConvertAR(ProductTotoal_withouttaxes.ToString());
                List.Add(Prnt_InvoiceCompany_Product);
            }
          
            Rpt_Invoice_Company report = new Rpt_Invoice_Company();
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
