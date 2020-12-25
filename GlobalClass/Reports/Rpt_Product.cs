using System;
using System.IO;
using DevExpress.XtraPrinting;
using Warehouse.Reports.Product;
using Warehouse.Models;
using System.Collections.Generic;
using Warehouse.Models.Reports;

namespace Warehouse.GlobalClass.Reports
{
    public class Rpt_Product
    {
        internal string ExportBarCodeReport(List<Prnt_ProductBarcode> Cls_Product)
        {

                string _path = System.Web.HttpContext.Current.Server.MapPath(@"~/Reports/pdf"); 
                Random random = new Random();
                string tick = DateTime.Now.Ticks.ToString();
                string reportPath = Path.Combine(_path, "BarCodeReport.pdf");

                BarCodeReport report = new BarCodeReport();
                report.DataSource = Cls_Product;

                PdfExportOptions pdfOptions = report.ExportOptions.Pdf;
                pdfOptions.Compressed = true;
                pdfOptions.ImageQuality = PdfJpegImageQuality.Low;
                pdfOptions.NeverEmbeddedFonts = "Tahoma;Courier New";
                pdfOptions.DocumentOptions.Application = "Human Resources Application";
                pdfOptions.DocumentOptions.Author = "مؤسسة الجود لتقنية المعلومات";
                pdfOptions.DocumentOptions.Subject = "باركود الصنف";
                pdfOptions.DocumentOptions.Title = "باركود الصنف";

                report.ExportToPdf(reportPath);
                return "BarCodeReport";
             
        }
    }
}