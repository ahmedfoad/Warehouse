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

using Warehouse.Models.Reports;
using Warehouse.Reports.Product;
using DevExpress.XtraPrinting;
using Warehouse.CustomFilters;
using Microsoft.AspNet.Identity;

namespace Warehouse.Controllers.Report
{
    public class Report_ProductController : Controller
    {
        DB_StoreEntities db = new DB_StoreEntities();
        private int recordsPerPage = 50;
        ErrorViewModel Error = new ErrorViewModel();

        [CustomAuthorize(Roles = "تقرير جرد المستودع&enter$1")]
        [HttpGet]
        public ActionResult AllProducts()
        {
            return View();
        }
        [CustomAuthorize]
        [HttpPost]
        public ActionResult AllProducts(int? page, Srch_Product Srch_Product)
        {
            var AllRecord = db.Products.AsQueryable();

            if (Srch_Product.Company_id != 0)
            {
                AllRecord = AllRecord.Where(x => x.Company_Id == Srch_Product.Company_id);
            }
            if (string.IsNullOrEmpty(Srch_Product.Product_Name) == false)
            {
                AllRecord = AllRecord.Where(x => x.Name.Contains(Srch_Product.Product_Name));
            }
            

            var skipRecords = ((page != null && page >= 1) ? page.Value : 0) * recordsPerPage;
            List<ClsProduct> RecordList = new List<ClsProduct>();
            var products = AllRecord
         .OrderBy(s => s.ID).Skip(skipRecords)
         .Take(recordsPerPage).ToList();
            foreach (var a in products)
            {
                ClsProduct ClsProduct = new ClsProduct
                {
                    ID = a.ID,
                    Name = a.Name,
                    Company_Name = a.Company.Name,

                    Price_Unit = a.Price_Unit,
                    Price_Mowrid = a.Price_Mowrid,
                    Taxes = a.Taxes,
                    Taxes_Price = Math.Round(((a.Price_Mowrid * (decimal)0.05)), 3, MidpointRounding.AwayFromZero),
                    TotalPrice = Math.Round(((a.Price_Mowrid) + (a.Price_Mowrid * (decimal)0.05)), 3, MidpointRounding.AwayFromZero)
                };

                Invoice_Company_Product Invoice_Company_Product = db.Invoice_Company_Product.Where(x => x.Product_Id == ClsProduct.ID).OrderByDescending(x => x.ID).FirstOrDefault();
                if (Invoice_Company_Product != null)
                {
                    ClsProduct.Price_Mowrid = Invoice_Company_Product.Price;
                    ClsProduct.TotalPrice = Math.Round(((ClsProduct.Price_Mowrid) + (ClsProduct.Price_Mowrid * (decimal)0.05)), 3, MidpointRounding.AwayFromZero);
                }
                RecordList.Add(ClsProduct);
            }


            if (Srch_Product.Price_From != 0 || Srch_Product.Price_To != 0)
            {
                RecordList = RecordList.Where(x => x.Price_Unit >= Srch_Product.Price_From && x.Price_Unit <= Srch_Product.Price_To).ToList();
            }
            if (Srch_Product.Price_Mowrid_From != 0 || Srch_Product.Price_Mowrid_To != 0)
            {
                RecordList = RecordList.Where(x => x.Price_Mowrid >= Srch_Product.Price_Mowrid_From && x.Price_Mowrid <= Srch_Product.Price_Mowrid_To).ToList();
            }
            var list = JsonConvert.SerializeObject(RecordList, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });

            return Content(list, "application/json");
        }


        [CustomAuthorize]
        [HttpGet]
        public ActionResult PdfProducts(Srch_Product Srch_Product)
        {
            ConvertToEasternArabicNumerals ConvertToEasternArabicNumerals = new ConvertToEasternArabicNumerals();
            List<Product> List = db.Products.ToList();
            List<Prnt_ProductSummary> DataSource = new List<Models.Reports.Prnt_ProductSummary>();
            
            foreach (var item in List)
            {
                Prnt_ProductSummary Cls_ProductSummary = new Prnt_ProductSummary();
                Cls_ProductSummary.Warehouse_Address = "الحدود الشمالية - محافظة رفحاء - طريق عمر بن الخطاب - حي الروضة";
                Cls_ProductSummary.Warehouse_Mobile = ConvertToEasternArabicNumerals.ConvertAR("0553390093");
                Cls_ProductSummary.Warehouse_Email = ConvertToEasternArabicNumerals.ConvertAR("naqirafha@gmail.com");
                int OldAmount = item.OldAmount;
                int Sell_Amount = db.Invoice_Mandob_Product.Where(a => a.Product_Id == item.ID && a.Invoice_Mandob.Is_Deleted == false).Select(a => a.Amount).DefaultIfEmpty(0).Sum();
                int Amount_Company = db.Invoice_Company_Product.Where(a => a.Product_Id == item.ID && a.Invoice_Company.Is_Deleted == false).Select(a => a.Amount).DefaultIfEmpty(0).Sum();
                int Prev_Amount = Amount_Company + OldAmount;
                int Current_Amount = Prev_Amount - Sell_Amount;
                Cls_ProductSummary.Product_Name = item.Name;
                Cls_ProductSummary.Company_Name = item.Company.Name;
                Cls_ProductSummary.Current_Amount = ConvertToEasternArabicNumerals.ConvertAR(Current_Amount.ToString());
                Cls_ProductSummary.Prev_Amount = ConvertToEasternArabicNumerals.ConvertAR(Prev_Amount.ToString());
                Cls_ProductSummary.Sell_Amount = ConvertToEasternArabicNumerals.ConvertAR(Sell_Amount.ToString());
                DataSource.Add(Cls_ProductSummary);
            }




            //Prnt_Masrofat.Bian = ConvertToEasternArabicNumerals.ConvertAR(Masrofat.Bian.ToString());


            Rpt_Products report = new Rpt_Products();
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