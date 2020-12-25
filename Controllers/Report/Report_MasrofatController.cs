using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Warehouse.CustomFilters;
using Warehouse.Models;
using Warehouse.Models.Administration;
using Warehouse.Models.Reports;
using Warehouse.GlobalClass;
using Warehouse.Reports.Product;
using DevExpress.XtraPrinting;
using System.IO;
using Warehouse.Reports.Eradat;
using Microsoft.AspNet.Identity;

namespace Warehouse.Controllers.Report
{
    public class Report_MasrofatController : Controller
    {
        DB_StoreEntities db = new DB_StoreEntities();
        private int recordsPerPage = 50;
        ErrorViewModel Error = new ErrorViewModel();
        [CustomAuthorize(Roles = "تقرير المصروفات&enter$1")]
        // GET: Search
        public ActionResult Masrofat()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Masrofat(int? page, Srch_Masrofat RModel)
        {
            var skipRecords = ((page != null && page >= 1) ? page.Value : 0) * recordsPerPage;
            var AllRecord = db.Masrofats.AsQueryable();
           
            if ((RModel.Date_Invoice_From != null && RModel.Date_Invoice_From != "" && RModel.Date_Invoice_TO != null && RModel.Date_Invoice_TO != ""))
            {
                DateTime Date_Invoice_From = DateTime.ParseExact(RModel.Date_Invoice_From, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime Date_Invoice_TO = DateTime.ParseExact(RModel.Date_Invoice_TO, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var endDateExclusive = Date_Invoice_TO.AddDays(1);
                AllRecord = AllRecord.Where(x => x.Date_Invoice >= Date_Invoice_From && x.Date_Invoice < endDateExclusive);
            }

            if (RModel.Money_From != 0 || RModel.Money_To != 0)
            {
                AllRecord = AllRecord.Where(x => x.Money >= RModel.Money_From && x.Money <= RModel.Money_To);
            }
            //.Where(s => string.IsNullOrEmpty(UserName) ? true : s.Name.Contains(UserName))
            var RecordList = AllRecord.OrderBy(s => s.ID).Skip(skipRecords)
             .Take(recordsPerPage).Select(a => new Cls_Masrofat
             {
                 ID = a.ID,
                 Invoice_Number = a.Invoice_Number,
                 Masrofat_Type_Id = a.Masrofat_Type_Id,
                 Masrofat_Type_Name = a.Masrofat_Type.Name,
                 _Date_Invoice = a.Date_Invoice,
                 Money = a.Money,
                 Bian = a.Bian
             }).ToList();
            var list = JsonConvert.SerializeObject(RecordList,
Formatting.None,
new JsonSerializerSettings()
{
    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
});

            return Content(list, "application/json");

        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult Pdf(string Date_Invoice_From, string Date_Invoice_TO)
        {
            ConvertToEasternArabicNumerals ConvertToEasternArabicNumerals = new ConvertToEasternArabicNumerals();
          
            List<Prnt_Masrofat> DataSource = new List<Models.Reports.Prnt_Masrofat>();
            var AllRecord = db.Masrofats.AsQueryable();

            if ((Date_Invoice_From != null && Date_Invoice_From != "" && Date_Invoice_TO != null && Date_Invoice_TO != ""))
            {
                DateTime _Date_Invoice_From = DateTime.ParseExact(Date_Invoice_From, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime _Date_Invoice_TO = DateTime.ParseExact(Date_Invoice_TO, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var endDateExclusive = _Date_Invoice_TO.AddDays(1);
                AllRecord = AllRecord.Where(x => x.Date_Invoice >= _Date_Invoice_From && x.Date_Invoice < endDateExclusive);
            }

            
            //.Where(s => string.IsNullOrEmpty(UserName) ? true : s.Name.Contains(UserName))
            var RecordList = AllRecord.OrderBy(s => s.ID).ToList();
            decimal Total_Money = RecordList.Sum(a => a.Money);
            foreach (var Masrofat in RecordList)
            {
                Prnt_Masrofat Prnt_Masrofat = new Prnt_Masrofat();
               Prnt_Masrofat.Warehouse_Address = "الحدود الشمالية - محافظة رفحاء - طريق عمر بن الخطاب - حي الروضة";
               Prnt_Masrofat.Warehouse_Mobile = ConvertToEasternArabicNumerals.ConvertAR("0553390093");
                Prnt_Masrofat.Warehouse_Email = "naqirafha@gmail.com";
                Prnt_Masrofat.Date_Invoice_From = ConvertToEasternArabicNumerals.ConvertAR(Date_Invoice_From);
                Prnt_Masrofat.Date_Invoice_TO = ConvertToEasternArabicNumerals.ConvertAR(Date_Invoice_TO);

                Prnt_Masrofat.ID = ConvertToEasternArabicNumerals.ConvertAR(Masrofat.ID.ToString());
                Prnt_Masrofat.Bian = ConvertToEasternArabicNumerals.ConvertAR(Masrofat.Bian.ToString());
                Prnt_Masrofat.Date_Invoice = ConvertToEasternArabicNumerals.ConvertAR(Masrofat.Date_Invoice.ToString("yyyy/MM/dd"));
                Prnt_Masrofat.Notes = ConvertToEasternArabicNumerals.ConvertAR(Masrofat.Notes);
                Prnt_Masrofat.Money = ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Masrofat.Money));
                Prnt_Masrofat.Invoice_Number = ConvertToEasternArabicNumerals.ConvertAR(Masrofat.Invoice_Number.ToString());
                Prnt_Masrofat.Masrofat_Type_Name = Masrofat.Masrofat_Type.Name;
                Prnt_Masrofat.Total_Money = ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Total_Money));
                DataSource.Add(Prnt_Masrofat);
            }




            //Prnt_Masrofat.Bian = ConvertToEasternArabicNumerals.ConvertAR(Masrofat.Bian.ToString());


            Rpt_MasrofatList report = new Rpt_MasrofatList();
            report.DataSource = DataSource;
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