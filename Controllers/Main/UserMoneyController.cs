using Warehouse.GlobalClass;
using Warehouse.Models;
using Warehouse.Models.Administration;
using Warehouse.Reports.Invoices;
using DevExpress.XtraPrinting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;

using System.Web;
using System.Web.Mvc;
using Warehouse.CustomFilters;
using Microsoft.AspNet.Identity;

namespace Warehouse.Controllers.Main
{
    public class UserMoneyController : Controller
    {
        ConvertToEasternArabicNumerals ConvertToEasternArabicNumerals = new ConvertToEasternArabicNumerals();
        DB_StoreEntities db = new DB_StoreEntities();
        ErrorViewModel Error = new ErrorViewModel();
        [CustomAuthorize]
        [HttpGet]
        public ActionResult Finish()
        {
            return View();
        }
        [CustomAuthorize]
        [HttpGet]
        public ActionResult LoadData()
        {
            ClsUser_Money ClsUser_Money = new ClsUser_Money();
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            User user = db.Users.Find(UserID);
            ClsUser_Money.User_Name = user.NAME;

          
          
            decimal Money = db.Invoice_Mandob_Sadad.Where(a => a.Invoice_Mandob.Is_Deleted == false && a.IS_Chekced == false).Select(a => a.Money).DefaultIfEmpty(0).Sum();
            ClsUser_Money.Money_Invoice_Mandob = Math.Round(Money, 2, MidpointRounding.AwayFromZero).ToString();
            var list = JsonConvert.SerializeObject(ClsUser_Money, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");

        }


        [HttpGet]
        public ActionResult loadUserMoney(int id)
        {
            ClsUser_Money ClsUser_Money = new ClsUser_Money();
            User_Money User_Money = db.User_Money.Find(id);
            if (User_Money != null)
            {

                ClsUser_Money = new ClsUser_Money
                {
                    ID = User_Money.ID.ToString(),
                    User_Id = User_Money.User_Id,
                    User_Name = User_Money.User.NAME,
                    Money_Invoice_Mandob = User_Money.Money_Invoice_Mandob.ToString(),
                    Money_Sarf = User_Money.Money_Sarf.ToString(),
                    Money_Sandok = User_Money.Money_Sandok.ToString(),
                    Money_Mada = User_Money.Money_Mada.ToString(),
                    Money_Remain = User_Money.Money_Remain.ToString(),
                    Save_Date = User_Money.Save_Date.ToString(),

                };


            }
            var list = JsonConvert.SerializeObject(ClsUser_Money,
           Formatting.None,
           new JsonSerializerSettings()
           {
               ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
           });
            return Content(list, "application/json");
        }
        [CustomAuthorize]
        [HttpPost]
        public ActionResult Insert(ClsUser_Money ClsUser_Money)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
       
            decimal Money_Invoice_Mandob = 0, Money_Sarf = 0, Money_Sandok = 0, Money_Mada = 0, Money_Remain = 0;
            decimal.TryParse(ClsUser_Money.Money_Invoice_Mandob, out Money_Invoice_Mandob);
            decimal.TryParse(ClsUser_Money.Money_Sarf, out Money_Sarf);
            decimal.TryParse(ClsUser_Money.Money_Sandok, out Money_Sandok);
            decimal.TryParse(ClsUser_Money.Money_Mada, out Money_Mada);
            decimal.TryParse(ClsUser_Money.Money_Remain, out Money_Remain);
            User_Money User_Money = new User_Money
            {
                User_Id = UserID,
                Money_Invoice_Mandob = Money_Invoice_Mandob,
                Money_Sarf = Money_Sarf,
                Money_Sandok = Money_Sandok,
                Money_Mada = Money_Mada,
                Money_Remain = Money_Remain,
                Save_Date = DateTime.Now
            };
            db.User_Money.Add(User_Money);
            db.SaveChanges();

            List<Invoice_Mandob_Sadad> Invoice_Mandob_Sadad = db.Invoice_Mandob_Sadad.Where(a => a.Invoice_Mandob.Is_Deleted == false && a.IS_Chekced == false).ToList();
            foreach (var item in Invoice_Mandob_Sadad)
            {
                item.IS_Chekced = true;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }


            //UserAction UserAction = new UserAction
            //{
            //    Userid = UserID,
            //    Viewid = viewid,
            //    ActionDate = DateTime.Now,
            //    Action = ((_Action)1).ToString(),//حفظ
            //    Operation = "حفظ تصفير الايرادات اليومية رقم : " + User_Money.ID
            //};
            //db.UserActions.Add(UserAction);
            //db.SaveChanges();
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = User_Money.ID;
            return Json(Error, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize]
        [HttpPost]
        public ActionResult Edit(ClsUser_Money ClsUser_Money)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
          
            int _id = int.Parse(ClsUser_Money.ID);
            User_Money User_Money = db.User_Money.Find(_id);
            User_Money.User_Id = UserID;
            User_Money.Money_Invoice_Mandob = decimal.Parse(ClsUser_Money.Money_Invoice_Mandob);
            User_Money.Money_Sarf = decimal.Parse(ClsUser_Money.Money_Sarf);
            User_Money.Money_Sandok = decimal.Parse(ClsUser_Money.Money_Sandok);
            User_Money.Money_Mada = decimal.Parse(ClsUser_Money.Money_Mada);
            User_Money.Money_Remain = decimal.Parse(ClsUser_Money.Money_Remain);
            User_Money.Save_Date = DateTime.Now;

            db.Entry(User_Money).State = EntityState.Modified;
            db.SaveChanges();




            //UserAction UserAction = new UserAction
            //{
            //    Userid = UserID,
            //    Viewid = viewid,
            //    ActionDate = DateTime.Now,
            //    Action = ((_Action)2).ToString(),//حفظ
            //    Operation = "تعديل تصفير الايرادات اليومية رقم : " + User_Money
            //};
            //db.UserActions.Add(UserAction);
            //db.SaveChanges();
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = User_Money.ID;
            return Json(Error, JsonRequestBehavior.AllowGet);
        }


        //[HttpGet]
        //public ActionResult PrintUserMoney(decimal id)
        //{
        //    ViewBag.MyUrl = "/UserMoney/PDF_Usermoney/" + id.ToString();
        //    return PartialView("PrintUserMoney");
        //}
        [HttpGet]
        public ActionResult PDF_Usermoney(int id)
        {
            List<ClsUser_Money> list = new List<ClsUser_Money>();
            User_Money User_Money = db.User_Money.Find(id);

            ClsUser_Money ClsUser_Money = new ClsUser_Money
            {
                ID = ConvertToEasternArabicNumerals.ConvertAR(User_Money.ID.ToString()),
                Money_Invoice_Mandob = ConvertToEasternArabicNumerals.ConvertAR(User_Money.Money_Invoice_Mandob.ToString()),
                Money_Sarf = ConvertToEasternArabicNumerals.ConvertAR(User_Money.Money_Sarf.ToString()),
                Money_Sandok = ConvertToEasternArabicNumerals.ConvertAR(User_Money.Money_Sandok.ToString()),
                Money_Mada = ConvertToEasternArabicNumerals.ConvertAR(User_Money.Money_Mada.ToString()),
                Money_Remain = ConvertToEasternArabicNumerals.ConvertAR(User_Money.Money_Remain.ToString()),
                Save_Date = ConvertToEasternArabicNumerals.ConvertAR(User_Money.Save_Date.ToString("tt hh:mm yyyy/MM/dd")),
                User_Name = User_Money.User.NAME
            };
            list.Add(ClsUser_Money);
            // Rpt_Product Rpt_Product = new Rpt_Product();

            //string _path = System.Web.HttpContext.Current.Server.MapPath(@"~/Reports/pdf");
            //Random random = new Random();
            //string tick = DateTime.Now.Ticks.ToString();
            //string reportPath = Path.Combine(_path, "BarCodeReport.pdf");

            Rpt_ZeroBox report = new Rpt_ZeroBox();
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
        //----------------------------------------
    }
}





















