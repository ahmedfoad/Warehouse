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
using Warehouse.CustomFilters;

namespace Warehouse.Controllers
{
    public class Eradat_TypeController : Controller
    {
        DB_StoreEntities db = new DB_StoreEntities();
        private int recordsPerPage = 20;
        ErrorViewModel Error = new ErrorViewModel();
        public ActionResult getAll(int? page, string Search)
        {
            int? skipRecords = (page != null ? page.Value : 0) * recordsPerPage;

            var AllRecords = db.Eradat_Type
              .Where(s => string.IsNullOrEmpty(Search) ? true : s.Name.Contains(Search))
              .OrderBy(a => a.Name).Skip(skipRecords != null ? skipRecords.Value : 0)
              .Take(recordsPerPage).Select(a => new Cls_Eradat_Type
              {
                  ID = a.ID,
                  Name = a.Name 
                  
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
        [CustomAuthorize(Roles = "نوع الايرادات&edit")]
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
        public ActionResult Edit(Eradat_Type Eradat_Type)
        {
            Eradat_Type item = db.Eradat_Type.Find(Eradat_Type.ID);
            item.Name = Eradat_Type.Name;
            
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
        public ActionResult Insert(Eradat_Type Eradat_Type)
        {
            db.Eradat_Type.Add(Eradat_Type);
            db.SaveChanges();
            Error.ErrorName = "تمت الحفظ بنجاح ... جاري إعادة تحميل الصفحة";
            return Json(Error, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Eradat_Type Eradat_Type = db.Eradat_Type.Find(id);
            db.Eradat_Type.Remove(Eradat_Type);
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