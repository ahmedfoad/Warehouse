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
    public class Invoice_MandobController : Controller
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
        public ActionResult Invoice_Mandob()
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
            Cls_Invoice_Mandob.Payment_Type = Invoice_Mandob.Payment_Type;
            if (Invoice_Mandob.Payment_Type == 1)
            {
                Cls_Invoice_Mandob.Payment_Type_Name = "كاش";
            }
            else if (Invoice_Mandob.Payment_Type == 2)
            {
                Cls_Invoice_Mandob.Payment_Type_Name = "اجل";
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

            foreach (var item in Invoice_Mandob.Invoice_Mandob_Product)
            {
                Cls_Invoice_Mandob.ClsInvoiceMandob_Product.Add(
                    new ClsInvoiceMandob_Product
                    {
                        ID = item.ID,
                        Invoice_Mandob_Id = item.Invoice_Mandob_Id,
                        Product_Id = item.Product_Id,
                        Product_Name = item.Product_Name,
                        Product_Name_Orginal = item.Product_Name,
                        Amount = item.Amount,
                        Price = item.Price,
                        Taxes = 0,//item.Taxes,
                        TotalPrice = item.TotalPrice,

                        Offer_BonusAmount = item.Offer_BonusAmount,
                        Offer_BonusAmount_Orginal = item.Product.Offer_BonusAmount,
                        Offer_Product_id = item.Offer_Product_id,
                        Offer_Product_id_Orginal = item.Product.Offer_Product_id,
                        Offer_Product_Name = (item.Product.Product2 != null) ? item.Product.Product2.Name : "",
                        Offer_Product_Name_Orginal = item.Product.Name
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
        public ActionResult AddItem(int index, Cls_Invoice_Mandob Cls_Invoice_Mandob)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());

            int Invoice_ID = 0, Invoice_Product_ID = 0, Invoice_Number = 0;
            System.Globalization.DateTimeFormatInfo HijriDTFI;
            HijriDTFI = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
            HijriDTFI.Calendar = new System.Globalization.HijriCalendar();
            HijriDTFI.ShortDatePattern = "dd/MM/yyyy";
            DateTime Date_Invoice = DateTime.ParseExact(Cls_Invoice_Mandob.Date_Invoice, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string Date_Invoice_Hijri = Date_Invoice.Date.ToString("dd/MM/yyyy", HijriDTFI);

            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();
            if (Cls_Invoice_Mandob.ID == 0)
            {
                Invoice_Number = db.Invoice_Mandob.Where(a => a.Is_Deleted == false).Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
                Invoice_Mandob _Invoice_Mandob = new Invoice_Mandob
                {
                    Invoice_Number = Invoice_Number,
                    Mandob_id = Cls_Invoice_Mandob.Mandob_id,
                    Date_Invoice = Date_Invoice,
                    Date_Invoice_Hijri = Date_Invoice_Hijri,
                    Price = Cls_Invoice_Mandob.Price,
                    Taxes = 0,//Cls_Invoice_Mandob.Taxes,
                    Total_Sadad = 0,
                    User_ID = UserID,
                    ComputerName = computerDetails[0],
                    ComputerUser = computerDetails[1],
                    InDate = DateTime.Now,
                    Payment_Type = Cls_Invoice_Mandob.Payment_Type
                };
                db.Invoice_Mandob.Add(_Invoice_Mandob);
                db.SaveChanges();
                Invoice_ID = _Invoice_Mandob.ID;
                Invoice_Mandob_Product Invoice_Mandob_Product;


                Invoice_Mandob_Product = new Invoice_Mandob_Product
                {
                    Invoice_Mandob_Id = _Invoice_Mandob.ID,
                    Product_Id = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[0].Product_Id,
                    Product_Name = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[0].Product_Name,
                    Amount = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[0].Amount,
                    Price = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[0].Price,
                    Taxes = 0,
                    TotalPrice = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[0].TotalPrice,
                };
                db.Invoice_Mandob_Product.Add(Invoice_Mandob_Product);
                db.SaveChanges();
                Invoice_Product_ID = Invoice_Mandob_Product.ID;
                //**************************************************************
                if (Cls_Invoice_Mandob.Payment_Type == 1) // كاش
                {
                    Cls_Invoice_Mandob.ID = _Invoice_Mandob.ID;
                    Save_Sadad(Cls_Invoice_Mandob);
                    Cls_Invoice_Mandob.Total_Sadad = Cls_Invoice_Mandob.Price;
                }
                //**************************************************************
            }
            else
            {
                Invoice_Mandob Invoice_Mandob = db.Invoice_Mandob.Find(Cls_Invoice_Mandob.ID);
                Invoice_Number = Invoice_Mandob.Invoice_Number;
                Invoice_Mandob.Mandob_id = Cls_Invoice_Mandob.Mandob_id;

                Invoice_Mandob.Date_Invoice = Date_Invoice;
                Invoice_Mandob.Date_Invoice_Hijri = Date_Invoice_Hijri;
                Invoice_Mandob.Price = Cls_Invoice_Mandob.Price;
                Invoice_Mandob.Taxes = 0;
                //*********************************************************************

                int Invoice_Id = Cls_Invoice_Mandob.ID;
                decimal Total_Sadad = db.Invoice_Mandob_Sadad.Where(a => a.Invoice_Id == Invoice_Id).Select(a => a.Money).DefaultIfEmpty(0).Sum();
                Invoice_Mandob.Total_Sadad = Total_Sadad;
                //*********************************************************************
                Invoice_Mandob.User_ID = UserID;
                Invoice_Mandob.ComputerName = computerDetails[0];
                Invoice_Mandob.ComputerUser = computerDetails[1];
                Invoice_Mandob.InDate = DateTime.Now;
                Invoice_Mandob.Payment_Type = Cls_Invoice_Mandob.Payment_Type;

                db.Entry(Invoice_Mandob).State = EntityState.Modified;
                db.SaveChanges();
                Invoice_ID = Invoice_Mandob.ID;

                Invoice_Mandob_Product Invoice_Mandob_Product = new Invoice_Mandob_Product
                {
                    Price_Mowrid = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Price_Mowrid,
                    Invoice_Mandob_Id = Invoice_Mandob.ID,
                    Product_Id = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Product_Id,
                    Amount = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Amount,
                    Price = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Price,
                    Product_Name = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Product_Name,
                    Taxes = 0,
                    TotalPrice = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].TotalPrice
                };
                db.Invoice_Mandob_Product.Add(Invoice_Mandob_Product);
                db.SaveChanges();

                Invoice_Product_ID = Invoice_Mandob_Product.ID;
                //**************************************************************
                if (Cls_Invoice_Mandob.Payment_Type == 1) // كاش
                {
                    Edit_Sadad(Cls_Invoice_Mandob);
                    Cls_Invoice_Mandob.Total_Sadad = Cls_Invoice_Mandob.Price;
                }
                //**************************************************************

            }

            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = Invoice_ID;
            Error.Invoice_Number = Invoice_Number;
            Error.Invoice_Product_ID = Invoice_Product_ID;
            Error.Total_Sadad = Cls_Invoice_Mandob.Total_Sadad;
            Error.index = index;
            
            return Json(Error, JsonRequestBehavior.AllowGet);

        }


        [CustomAuthorize]
        [HttpPost]
        public ActionResult EditItem(int index, int Invoice_Mandob_Product_ID, Cls_Invoice_Mandob Cls_Invoice_Mandob)
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

            Invoice_Mandob_Product Invoice_Mandob_Product = db.Invoice_Mandob_Product.Find(Invoice_Mandob_Product_ID);

            Invoice_Mandob_Product.Invoice_Mandob_Id = Invoice_Mandob.ID;
            Invoice_Mandob_Product.Product_Id = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Product_Id;
            //***************************************************************************
            Invoice_Mandob_Product.Price_Mowrid = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Price_Mowrid;
            // يتغير اسم الصنف مع اضافة صنف العرض عند وصول للتارجيت
            Invoice_Mandob_Product.Product_Name = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Product_Name;
            Invoice_Mandob_Product.Offer_Product_id = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_Product_id;
            Invoice_Mandob_Product.Offer_BonusAmount = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_BonusAmount;
            //***************************************************************************

            Invoice_Mandob_Product.Amount = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Amount;
            Invoice_Mandob_Product.Price = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Price;
            //Invoice_Mandob_Product.Product_Name = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Product_Name;
            Invoice_Mandob_Product.Taxes = 0;
            Invoice_Mandob_Product.TotalPrice = Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].TotalPrice;
            db.Entry(Invoice_Mandob_Product).State = EntityState.Modified;
            db.SaveChanges();

            //**************************************************************
            if (Cls_Invoice_Mandob.Payment_Type == 1) // كاش
            {
                Edit_Sadad(Cls_Invoice_Mandob);
                Cls_Invoice_Mandob.Total_Sadad = Cls_Invoice_Mandob.Price;
            }
            //**************************************************************
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = Invoice_Mandob.ID;
            Error.Invoice_Product_ID = Invoice_Mandob_Product_ID;
            Error.Total_Sadad = Cls_Invoice_Mandob.Total_Sadad;
            Error.index = index;
            return Json(Error, JsonRequestBehavior.AllowGet);

        }

        public void Save_Sadad(Cls_Invoice_Mandob Cls_Invoice_Mandob)
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
                Invoice_Id = Cls_Invoice_Mandob.ID,
                Sadad_Type_Id = 1,
                Date_Added = Date_Invoice,
                Money = Cls_Invoice_Mandob.Price,
                //Remain = ClsInvoice_Mandob_Sadad.Remain,

                User_ID = UserID,
                ComputerName = computerDetails[0],
                ComputerUser = computerDetails[1],
                InDate = Date_Invoice
            };
            db.Invoice_Mandob_Sadad.Add(Sadad);
            db.SaveChanges();
            int Invoice_Id = Cls_Invoice_Mandob.ID;
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
        }

        public ActionResult Edit_Sadad(Cls_Invoice_Mandob Cls_Invoice_Mandob)
        {
            DateTime Date_Invoice = DateTime.Now;
            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            //Add New
            int Invoice_Id = Cls_Invoice_Mandob.ID;
            Invoice_Mandob_Sadad Invoice_Mandob_Sadad = db.Invoice_Mandob_Sadad.Where(a => a.Invoice_Id == Invoice_Id).FirstOrDefault();
            Invoice_Mandob_Sadad.Sadad_Type_Id = 1;
            Invoice_Mandob_Sadad.Date_Added = Date_Invoice;
            Invoice_Mandob_Sadad.Money = Cls_Invoice_Mandob.Price;
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
            Eradat Eradat = db.Eradats.Where(a => a.Invoice_Mandob_Sadad_ID == Invoice_Mandob_Sadad.ID).FirstOrDefault();

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