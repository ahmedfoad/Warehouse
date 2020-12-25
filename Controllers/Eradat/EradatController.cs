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

    public class EradatController : Controller
    {
        DB_StoreEntities db = new DB_StoreEntities();
        private int recordsPerPage = 20;
        ErrorViewModel Error = new ErrorViewModel();


        [HttpGet]
        public ActionResult loadEradat(int id)
        {
            Cls_Eradat Cls_Eradat = new Cls_Eradat();
            Eradat Eradat = db.Eradats.Find(id);
            if (Eradat != null)
            {
                if (Eradat.Invoice_Mandob_Sadad_ID != null)
                {
                    Error.ErrorFullNumber = "AR-Open-000";
                    Error.ErrorNumber = "000";
                    Error.Url = "/Home";
                    Error.ErrorName = "ليس لديك صلاحية الدخول ";
                    return View("~/Views/Shared/ErrorPage.cshtml", Error);
                }
                Cls_Eradat.ID = Eradat.ID;
                Cls_Eradat.Invoice_Number = Eradat.Invoice_Number;
                Cls_Eradat.Date_Invoice = Eradat.Date_Invoice.ToString("yyyy-MM-dd");
                Cls_Eradat.Eradat_Type_Id = Eradat.Eradat_Type_Id;
                Cls_Eradat.Eradat_Type_Name = Eradat.Eradat_Type.Name;
                Cls_Eradat.Money = Eradat.Money;
                Cls_Eradat.Bian = Eradat.Bian;
                Cls_Eradat.Notes = Eradat.Notes;
                Cls_Eradat.InDate = Eradat.InDate;
                Cls_Eradat.User_Name = Eradat.User1.NAME;
                Cls_Eradat.ComputerName = Eradat.ComputerName;
                Cls_Eradat.ComputerUser = Eradat.ComputerUser;
            }

            var list = JsonConvert.SerializeObject(Cls_Eradat, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
        }
        [HttpGet]
        [CustomAuthorize(Roles = "اضافة ايرادات جديدة&edit")]
        public ActionResult Operation()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetEradat_Type()
        {
            return PartialView();
        }
        public ActionResult getAllEradat_Type(int? page, string Search)
        {
            int? skipRecords = (page != null ? page.Value : 0) * recordsPerPage;

            var AllRecords = db.Eradat_Type.Select(a => new Cls_Eradat_Type
            {
                ID = a.ID,
                Name = a.Name,
            })
              .Where(s => s.ID > 1 && (string.IsNullOrEmpty(Search) ? true : s.Name.Contains(Search)))
              .OrderBy(s => s.ID).Skip(skipRecords != null ? skipRecords.Value : 0)
              .Take(recordsPerPage).ToList();
            var list = JsonConvert.SerializeObject(AllRecords, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
        }


        [HttpPost]
        [CustomAuthorize]
        public ActionResult Insert(Cls_Eradat Cls_Eradat)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            DateTime Date_Invoice = DateTime.ParseExact(Cls_Eradat.Date_Invoice, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();
            int Invoice_Number = db.Eradats.Where(a => a.Is_Deleted == false).Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
            Eradat Eradat = new Eradat
            {
                Invoice_Number = Invoice_Number,
                Date_Invoice = Date_Invoice,
                Money = Cls_Eradat.Money,
                Eradat_Type_Id = Cls_Eradat.Eradat_Type_Id,
                Bian = Cls_Eradat.Bian,
                Notes = Cls_Eradat.Notes,
                Userid_In = UserID,
                InDate = DateTime.Now,
                ComputerName = computerDetails[0],
                ComputerUser = computerDetails[1]
            };
            db.Eradats.Add(Eradat);
            db.SaveChanges();

            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = Eradat.ID;
            return Json(Error, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Edit(Cls_Eradat Cls_Eradat)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            DateTime Date_Invoice = DateTime.ParseExact(Cls_Eradat.Date_Invoice, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            Eradat item = db.Eradats.Find(Cls_Eradat.ID);
            if (item.Invoice_Mandob_Sadad_ID != null)
            {
                Error.ErrorFullNumber = "AR-Open-000";
                Error.ErrorNumber = "000";
                Error.Url = "/Home";
                Error.ErrorName = "ليس لديك صلاحية الدخول ";
                return View("~/Views/Shared/ErrorPage.cshtml", Error);
            }
            item.Date_Invoice = Date_Invoice;
            item.Money = Cls_Eradat.Money;
            item.Eradat_Type_Id = Cls_Eradat.Eradat_Type_Id;
            item.Bian = Cls_Eradat.Bian;
            item.Notes = Cls_Eradat.Notes;
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
            List<Prnt_Eradat> List = new List<Prnt_Eradat>();
            Eradat Eradat = db.Eradats.Find(id);
         
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

    }
}