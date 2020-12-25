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

namespace Warehouse.Controllers
{
    public class OmolaMandobController : Controller
    {
        ConvertToEasternArabicNumerals ConvertToEasternArabicNumerals = new ConvertToEasternArabicNumerals();
        DB_StoreEntities db = new DB_StoreEntities();
        ErrorViewModel Error = new ErrorViewModel();
        #region Omola Mandobs
        [CustomAuthorize(Roles = "صرف عمولة المندوب&save$1")]
        // GET: Search
        public ActionResult Omola()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Omola(Srch_Sadad_MandobsList RModel)
        {
            //var roundedD = Math.Round(2.5, 0); // Output: 2
            //var roundedE = Math.Round(2.5, 0, MidpointRounding.AwayFromZero); // Output: 3

            var AllRecord = db.Invoice_Mandob.Where(a=> a.Is_Deleted == false).AsQueryable();
            if (RModel.Mandob_id != 0)
            {
                AllRecord = AllRecord.Where(x => x.Mandob_id == RModel.Mandob_id);
            }
            if (RModel.Sadad_Status == 1)//لم يتم السداد
            {
                AllRecord = AllRecord.Where(x => x.IS_Payed_Omola == false);
            }
            if (RModel.Sadad_Status == 2)//تم السداد كله
            {
                AllRecord = AllRecord.Where(x => x.IS_Payed_Omola == true);
            }
            //&& a.IS_Payed_Omola == false && a.Is_Deleted == false
            List<Invoice_Mandob> Invoice_Mandobs = AllRecord.ToList();

            List<Mandob> Mandobs = db.Mandobs.ToList();
            List<Cls_Invoice_Mandob> RecordList = new List<Cls_Invoice_Mandob>();
            Omola Omola = db.Omolas.FirstOrDefault();
            
         
            foreach (var item in Mandobs)
            {
                if (item.ID == 3)
                { 
}
                int Amount = 0;
                decimal Omola_Money_Orignal = 0;
                decimal? Omola_Money = 0;
                decimal Total_Price = 0;
                string Omola_Text = "";
                decimal Remain = 0;
                //Amount = Invoice_Mandobs.Where(a => a.Mandob_id == item.ID).Select(a => a.Amount).DefaultIfEmpty(0).Sum();
                //Amount = Invoice_Mandobs.Where(a => a.Mandob_id == item.ID).Select(a => a.Invoice_Mandob_Product.ToList().Select(a=>a.Amount).DefaultIfEmpty(0).Sum());
                Amount = Invoice_Mandobs.Where(a => a.Mandob_id == item.ID).SelectMany(a=>a.Invoice_Mandob_Product).GroupBy(a=>a.ID).Select(grp => new {
                           ID = grp.Key,
                           Sum = grp.Sum(t=>t.Amount)}).Select(x=>x.Sum).DefaultIfEmpty(0).Sum();
                Total_Price = Invoice_Mandobs.Where(a => a.Mandob_id == item.ID).Select(a => a.Price).DefaultIfEmpty(0).Sum();
                Omola_Money = Invoice_Mandobs.Where(a => a.Mandob_id == item.ID).Select(a => a.Omola_Money).DefaultIfEmpty(0).Sum();

                if (Omola.Omola_Type == false)//" ريال لكل صنف ";
                {
                    Omola_Text = Omola.Omola_quota + " ريال لكل صنف ";
                    Omola_Money_Orignal = Amount * Omola.Omola_quota;
                }
                else // + " % " + " نسبة على كل صنف";
                {
                    Omola_Text = Omola.Omola_quota + " % " + " نسبة على كل صنف";
                    Omola_Money_Orignal = ((Omola.Omola_quota / 100) * Total_Price) + Total_Price;

                }
                Remain = Omola_Money_Orignal - Omola_Money ?? default(decimal);
                Cls_Invoice_Mandob Cls_Invoice_Mandob = new Cls_Invoice_Mandob
                {
                    Mandob_id = item.ID,
                    Mandob_Name = item.Name,
                    Price = Omola_Money_Orignal,
                    Omola_Text = Omola_Text,
                    Omola_Money_Orignal = Omola_Money_Orignal,
                    Omola_Amount_AllProducts = Amount,
                    Total_Sadad= Omola_Money ?? default(decimal),
                    Remain= Remain
                };

                RecordList.Add(Cls_Invoice_Mandob);
            }
            var list = JsonConvert.SerializeObject(RecordList,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });
            return Content(list, "application/json");
        }

        [HttpGet]
        public ActionResult popup_OmolaMandob()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult Omola_Confirm()
        {
            return PartialView();
        }

        [CustomAuthorize]
        [HttpPost]
        public ActionResult SaveList(Cls_Mandob_SadadList Cls_Mandob_SadadList)
        {
            List<Invoice_Mandob> Invoice_MandobList = db.Invoice_Mandob.Where(x => x.Mandob_id == Cls_Mandob_SadadList.Mandob_id && x.Price == x.Total_Sadad && x.IS_Payed_Omola == false).ToList();
            DateTime Date_Invoice = DateTime.Now;
            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());

            int Amount = 0;
            decimal Remain_Money = Cls_Mandob_SadadList.Money;
            decimal Current_Money = 0;
         
            decimal  Omola_Money_Orignal=0;
            
            Omola Omola = db.Omolas.FirstOrDefault();
            foreach (var item in Invoice_MandobList)
            {
                if (Remain_Money <= 0)
                {
                    //return;
                    //continue;   // Skip the remainder of this iteration.
                    break;
                }
                int Invoice_Id = item.ID;
                Invoice_Mandob Invoice_Mandob = db.Invoice_Mandob.Find(Invoice_Id);
                Amount = item.Invoice_Mandob_Product.Select(a => a.Amount).DefaultIfEmpty(0).Sum();
                if (Omola.Omola_Type == false)//" ريال لكل صنف ";
                {
                    
                    Omola_Money_Orignal = Amount * Omola.Omola_quota;
                }
                else // + " % " + " نسبة على كل صنف";
                {
                    Omola_Money_Orignal = ((Omola.Omola_quota / 100) * item.Price) + item.Price;

                }
                decimal _Omola_Money = Math.Round(item.Omola_Money ?? default(decimal), 0, MidpointRounding.AwayFromZero);
                if (Omola_Money_Orignal == _Omola_Money)
                {
                    item.IS_Payed_Omola = true;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    continue;   // Skip the remainder of this iteration.
                }
                Current_Money = Omola_Money_Orignal - (item.Omola_Money ?? default(decimal));
                if (Current_Money > Remain_Money)
                {
                    Current_Money = Remain_Money;
                    Remain_Money = 0;
                }
                else if (Current_Money <= Remain_Money)
                {
                    if (Current_Money == Remain_Money)
                    {
                        Invoice_Mandob.IS_Payed_Omola = true;
                    }
                    Remain_Money = Remain_Money - Current_Money;
                }
                //*********************************************************************
                Invoice_Mandob.Omola_Money = Invoice_Mandob.Omola_Money + Current_Money;
                Invoice_Mandob.Omola_Type = Omola.Omola_Type;
                Invoice_Mandob.Omola_quota = Omola.Omola_quota;
                db.Entry(Invoice_Mandob).State = EntityState.Modified;
                db.SaveChanges();
                //*********************************************************************
                int Invoice_Number = db.Masrofats.Where(a => a.Is_Deleted == false).Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
                Masrofat Masrofat = new Masrofat
                {
                    Invoice_Number = Invoice_Number,
                    Invoice_Mandob_ID = Invoice_Mandob.ID,
                    Date_Invoice = Date_Invoice,
                    Masrofat_Type_Id = 2, // صرف عمولة مبيعات
                    Money = Cls_Mandob_SadadList.Money,
                    Bian = "سداد عمولة مندوب مبيعات لفاتورة مبيعات رقم  " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Invoice_Mandob.Invoice_Number.ToString())) + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Invoice_Mandob.Date_Invoice.ToString("yyyy/MM/dd"))
                       + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Cls_Mandob_SadadList.Money))
                       + " ريال ",
                    Userid_In = UserID,
                    ComputerName = computerDetails[0],
                    ComputerUser = computerDetails[1],
                    InDate = Date_Invoice
                };
                db.Masrofats.Add(Masrofat);
                db.SaveChanges();
                //*********************************************************************





            }

            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
          
            // return Json(Error, JsonRequestBehavior.AllowGet);
            var list = JsonConvert.SerializeObject(Error, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
        }

        #endregion
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