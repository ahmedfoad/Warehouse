using Warehouse.Models;
using Warehouse.Models.Administration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Warehouse.CustomFilters;
using Warehouse.GlobalClass;
using Warehouse.Models.Reports;
using DevExpress.XtraPrinting;
using System.IO;
using Warehouse.Reports.Eradat;


namespace Warehouse.Controllers
{
    [CustomAuthorize]
    public class MasrofatController : Controller
    {
        DB_StoreEntities db = new DB_StoreEntities();
        private int recordsPerPage = 20;
        ErrorViewModel Error = new ErrorViewModel();


        [HttpGet]
        public ActionResult loadMasrofat(int id)
        {
            Cls_Masrofat Cls_Masrofat = new Cls_Masrofat();
            Masrofat Masrofat = db.Masrofats.Find(id);
            if (Masrofat != null)
            {
                if (Masrofat.Invoice_Company_Sadad_ID != null || Masrofat.Invoice_Mandob_ID != null || Masrofat.Salary_ID != null)
                {
                    Error.ErrorFullNumber = "AR-Open-000";
                    Error.ErrorNumber = "000";
                    Error.Url = "/Home";
                    Error.ErrorName = "ليس لديك صلاحية الدخول ";
                    return View("~/Views/Shared/ErrorPage.cshtml", Error);
                }
                Cls_Masrofat.ID = Masrofat.ID;
                Cls_Masrofat.Invoice_Number = Masrofat.Invoice_Number;
                Cls_Masrofat.Date_Invoice = Masrofat.Date_Invoice.ToString("yyyy-MM-dd");
                Cls_Masrofat.Masrofat_Type_Id = Masrofat.Masrofat_Type_Id;
                Cls_Masrofat.Masrofat_Type_Name = Masrofat.Masrofat_Type.Name;
                Cls_Masrofat.Money = Masrofat.Money;
                Cls_Masrofat.Bian = Masrofat.Bian;
                Cls_Masrofat.Notes = Masrofat.Notes;
                Cls_Masrofat.InDate = Masrofat.InDate;
                Cls_Masrofat.User_Name = Masrofat.User.NAME;
                Cls_Masrofat.ComputerName = Masrofat.ComputerName;
                Cls_Masrofat.ComputerUser = Masrofat.ComputerUser;
            }

            var list = JsonConvert.SerializeObject(Cls_Masrofat, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
        }
        [HttpGet]
        [CustomAuthorize(Roles = "اضافة مصروفات جديدة&edit")]
        public ActionResult Operation()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetMasrofat_Type()
        {
            return PartialView();
        }
        public ActionResult getAllMasrofat_Type(int? page, string Search)
        {
            int? skipRecords = (page != null ? page.Value : 0) * recordsPerPage;

            var AllRecords = db.Masrofat_Type.Select(a => new Cls_Masrofat_Type
            {
                ID = a.ID,
                Name = a.Name,
            })
              .Where(s => s.ID > 3 && (string.IsNullOrEmpty(Search) ? true : s.Name.Contains(Search)))
              .OrderBy(s => s.ID).Skip(skipRecords != null ? skipRecords.Value : 0)
              .Take(recordsPerPage).ToList();
            var list = JsonConvert.SerializeObject(AllRecords, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
        }


        [HttpPost]
        [CustomAuthorize]
        public ActionResult Insert(Cls_Masrofat Cls_Masrofat)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            DateTime Date_Invoice = DateTime.ParseExact(Cls_Masrofat.Date_Invoice, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();
            int Invoice_Number = db.Masrofats.Where(a => a.Is_Deleted == false).Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
            Masrofat Masrofat = new Masrofat
            {
                Invoice_Number= Invoice_Number,
                Date_Invoice = Date_Invoice,
                Money=Cls_Masrofat.Money,
                Masrofat_Type_Id = Cls_Masrofat.Masrofat_Type_Id,
                Bian = Cls_Masrofat.Bian,
                Notes = Cls_Masrofat.Notes,
                Userid_In = UserID,
                InDate = DateTime.Now,
                ComputerName = computerDetails[0],
                ComputerUser = computerDetails[1]
            };
            db.Masrofats.Add(Masrofat);
            db.SaveChanges();

            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = Masrofat.ID;
            return Json(Error, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Edit(Cls_Masrofat Cls_Masrofat)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            DateTime Date_Invoice = DateTime.ParseExact(Cls_Masrofat.Date_Invoice, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            Masrofat item = db.Masrofats.Find(Cls_Masrofat.ID);
            if (item.Invoice_Company_Sadad_ID != null || item.Invoice_Mandob_ID != null || item.Salary_ID != null)
            {
                Error.ErrorFullNumber = "AR-Open-000";
                Error.ErrorNumber = "000";
                Error.Url = "/Home";
                Error.ErrorName = "ليس لديك صلاحية الدخول ";
                return View("~/Views/Shared/ErrorPage.cshtml", Error);
            }
            item.Date_Invoice = Date_Invoice;
            item.Money = Cls_Masrofat.Money;
            item.Masrofat_Type_Id = Cls_Masrofat.Masrofat_Type_Id;
            item.Bian = Cls_Masrofat.Bian;
            item.Notes = Cls_Masrofat.Notes;
            item.userid_Edit = UserID;
            item.EditDate = DateTime.Now;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            Error.ErrorName = "تمت الحفظ بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = item.ID;
            return Json(Error, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult PdfInvoice(int id = 0)
        {
            ConvertToEasternArabicNumerals ConvertToEasternArabicNumerals = new ConvertToEasternArabicNumerals();
            List<Prnt_Masrofat> List = new List<Prnt_Masrofat>();
            Masrofat Masrofat = db.Masrofats.Find(id);

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

    }
}