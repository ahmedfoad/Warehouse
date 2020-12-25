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
    public class Report_ArbahController : Controller
    {
        // GET: Report_Arbah
        DB_StoreEntities db = new DB_StoreEntities();
        private int recordsPerPage = 50;
        ErrorViewModel Error = new ErrorViewModel();
        [CustomAuthorize(Roles = "تقرير الارباح&enter$1")]
        // GET: Search
        public ActionResult Arbah()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Arbah(int? page, Srch_Eradat RModel)
        {
            var skipRecords = ((page != null && page >= 1) ? page.Value : 0) * recordsPerPage;
            var AllRecord_Eradat = db.Eradats.AsQueryable();
            var AllRecord_Masrofat = db.Masrofats.AsQueryable();

            if ((RModel.Date_Invoice_From != null && RModel.Date_Invoice_From != "" && RModel.Date_Invoice_TO != null && RModel.Date_Invoice_TO != ""))
            {
                DateTime Date_Invoice_From = DateTime.ParseExact(RModel.Date_Invoice_From, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime Date_Invoice_TO = DateTime.ParseExact(RModel.Date_Invoice_TO, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var endDateExclusive = Date_Invoice_TO.AddDays(1);
                AllRecord_Eradat = AllRecord_Eradat.Where(x => x.Date_Invoice >= Date_Invoice_From && x.Date_Invoice < endDateExclusive);
                AllRecord_Masrofat = AllRecord_Masrofat.Where(x => x.Date_Invoice >= Date_Invoice_From && x.Date_Invoice < endDateExclusive);
            }

            List<Cls_Arbah> RecordList_Eradat = new List<Models.Cls_Arbah>();
            List<Cls_Arbah> RecordList_Masrofat = new List<Models.Cls_Arbah>();
            List<Cls_Arbah> RecordList = new List<Models.Cls_Arbah>();
            //.Where(s => string.IsNullOrEmpty(UserName) ? true : s.Name.Contains(UserName))
            RecordList_Eradat = AllRecord_Eradat.OrderBy(s => s.ID).Skip(skipRecords)
             .Take(recordsPerPage).Select(a => new Cls_Arbah
             {
                  Type_Name = a.Eradat_Type.Name,
                 _Date_Invoice = a.Date_Invoice,
                 Money_Eradat = a.Money,
                 Bian = a.Bian
             }).ToList();

            RecordList_Masrofat = AllRecord_Masrofat.OrderBy(s => s.ID).Skip(skipRecords)
             .Take(recordsPerPage).Select(a => new Cls_Arbah
             {
                 Type_Name = a.Masrofat_Type.Name,
                 _Date_Invoice = a.Date_Invoice,
                 Money_Masrofat = a.Money,
                 Bian = a.Bian
             }).ToList();
            RecordList.AddRange(RecordList_Eradat);
            RecordList.AddRange(RecordList_Masrofat);
            RecordList = RecordList.OrderBy(a => a._Date_Invoice).ToList();
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

            List<Prnt_Arbah> DataSource = new List<Models.Reports.Prnt_Arbah>();
       
            var AllRecord_Eradat = db.Eradats.AsQueryable();
            var AllRecord_Masrofat = db.Masrofats.AsQueryable();
            if ((Date_Invoice_From != null && Date_Invoice_From != "" && Date_Invoice_TO != null && Date_Invoice_TO != ""))
            {
                DateTime _Date_Invoice_From = DateTime.ParseExact(Date_Invoice_From, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime _Date_Invoice_TO = DateTime.ParseExact(Date_Invoice_TO, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var endDateExclusive = _Date_Invoice_TO.AddDays(1);
                AllRecord_Eradat = AllRecord_Eradat.Where(x => x.Date_Invoice >= _Date_Invoice_From && x.Date_Invoice < endDateExclusive);
                AllRecord_Masrofat = AllRecord_Masrofat.Where(x => x.Date_Invoice >= _Date_Invoice_From && x.Date_Invoice < endDateExclusive);
            }
            List<Cls_Arbah> RecordList_Eradat = new List<Models.Cls_Arbah>();
            List<Cls_Arbah> RecordList_Masrofat = new List<Models.Cls_Arbah>();
            List<Cls_Arbah> RecordList = new List<Models.Cls_Arbah>();
            RecordList_Eradat = AllRecord_Eradat.OrderBy(s => s.ID)
           .Take(recordsPerPage).Select(a => new Cls_Arbah
           {
               Type_Name = a.Eradat_Type.Name,
               _Date_Invoice = a.Date_Invoice,
               Money_Eradat = a.Money,
               Bian = a.Bian
           }).ToList();

            RecordList_Masrofat = AllRecord_Masrofat.OrderBy(s => s.ID)
             .Take(recordsPerPage).Select(a => new Cls_Arbah
             {
                 Type_Name = a.Masrofat_Type.Name,
                 _Date_Invoice = a.Date_Invoice,
                 Money_Masrofat = a.Money,
                 Bian = a.Bian
             }).ToList();
            RecordList.AddRange(RecordList_Eradat);
            RecordList.AddRange(RecordList_Masrofat);
            RecordList = RecordList.OrderBy(a => a._Date_Invoice).ToList();
            decimal Total_Money_Eradat = RecordList.Select(a => a.Money_Eradat).DefaultIfEmpty(0).Sum();
            decimal Total_Money_Masrofat = RecordList.Select(a => a.Money_Masrofat).DefaultIfEmpty(0).Sum();
            //.Where(s => string.IsNullOrEmpty(UserName) ? true : s.Name.Contains(UserName))
           
            
            foreach (var item in RecordList)
            {
                Prnt_Arbah Prnt_Arbah = new Prnt_Arbah();
                string x = (RecordList.IndexOf(item) + 1).ToString();
                Prnt_Arbah.Index = ConvertToEasternArabicNumerals.ConvertAR(x);
                Prnt_Arbah.Warehouse_Address = "الحدود الشمالية - محافظة رفحاء - طريق عمر بن الخطاب - حي الروضة";
               Prnt_Arbah.Warehouse_Mobile = ConvertToEasternArabicNumerals.ConvertAR("0553390093");
               Prnt_Arbah.Warehouse_Email = "naqirafha@gmail.com";
               Prnt_Arbah.Date_Invoice_From = ConvertToEasternArabicNumerals.ConvertAR(Date_Invoice_From);
                Prnt_Arbah.Date_Invoice_TO = ConvertToEasternArabicNumerals.ConvertAR(Date_Invoice_TO);

              
                Prnt_Arbah.Bian = ConvertToEasternArabicNumerals.ConvertAR(item.Bian.ToString());
                Prnt_Arbah.Date_Invoice = ConvertToEasternArabicNumerals.ConvertAR(item._Date_Invoice.ToString("yyyy/MM/dd"));
                
                Prnt_Arbah.Money_Eradat = ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", item.Money_Eradat));
                Prnt_Arbah.Money_Masrofat = ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", item.Money_Masrofat));
                Prnt_Arbah.Type_Name = item.Type_Name;
                Prnt_Arbah.Total_Eradat = ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Total_Money_Eradat));
                Prnt_Arbah.Total_Masrofat = ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Total_Money_Masrofat));
                Prnt_Arbah.Total_Arbah = ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", (Total_Money_Eradat - Total_Money_Masrofat)));
                DataSource.Add(Prnt_Arbah);
            }




            //Prnt_Masrofat.Bian = ConvertToEasternArabicNumerals.ConvertAR(Masrofat.Bian.ToString());


            Rpt_ArbahList report = new Rpt_ArbahList();
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