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

namespace Warehouse.Controllers.Tarmeez
{
    public class MandobController : Controller
    {
        DB_StoreEntities db = new DB_StoreEntities();
        private int recordsPerPage = 20;
        ErrorViewModel Error = new ErrorViewModel();
        public ActionResult getAll(int? page, string Search)
        {
            int? skipRecords = (page != null ? page.Value : 0) * recordsPerPage;

            var AllRecords = db.Mandobs
              .Where(s => string.IsNullOrEmpty(Search) ? true : s.Name.Contains(Search))
              .OrderBy(a => a.Name).Skip(skipRecords != null ? skipRecords.Value : 0)
              .Take(recordsPerPage).Select(a => new Cls_Mandob
              {
                  ID = a.ID,
                  Name = a.Name,
                  JawalNO=a.JawalNO,
                  Address=a.Address

              }).ToList();

            var list = JsonConvert.SerializeObject(AllRecords,
    Formatting.None,
    new JsonSerializerSettings()
    {
        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    });

            return Content(list, "application/json");
            //return Json(AllRecords, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Edit(Mandob Mandob)
        {
            Mandob item = db.Mandobs.Find(Mandob.ID);
            item.Name = Mandob.Name;
            item.JawalNO = Mandob.JawalNO;
            item.Address = Mandob.Address;

            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            Error.ErrorName = "تمت الحفظ بنجاح ... جاري إعادة تحميل الصفحة";
            return Json(Error, JsonRequestBehavior.AllowGet);
        }

        //جلب الامنتجات لفاتورة مندوب المبيعات----------------------
        [HttpGet]
        public ActionResult GetProducts()
        {
            return PartialView();
        }
        //البحث عن صنف مندوب المبيعات----------------------
        [HttpGet]
        public ActionResult GetSearchProducts()
        {
            return PartialView();
        }


        [HttpPost]
        public ActionResult Insert(Mandob Mandob)
        {
            if (db.Mandobs.Where(a => a.Name == Mandob.Name).Any() == false)
            {
                int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
                WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
                List<string> computerDetails = identity.Name.Split('\\').ToList();
                Mandob.User_ID = UserID;
                Mandob.InDate = DateTime.Now;
                Mandob.ComputerName = computerDetails[0];
                Mandob.ComputerUser = computerDetails[1];
                db.Mandobs.Add(Mandob);
                db.SaveChanges();
                Error.ErrorName = "تمت الحفظ بنجاح ... جاري إعادة تحميل الصفحة";
                return Json(Error, JsonRequestBehavior.AllowGet);
            }
            Error.ErrorName = "لم يتم الحفظ لتكرار الاسم";
            return Json(Error, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Mandob Mandob = db.Mandobs.Find(id);
            db.Mandobs.Remove(Mandob);
            db.SaveChanges();
            Error.ErrorName = "تمت الحذف بنجاح ... جاري إعادة تحميل الصفحة";
            return Json(Error, JsonRequestBehavior.AllowGet);


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