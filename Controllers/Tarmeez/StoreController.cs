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

namespace Warehouse.Controllers.Tarmeez
{
    public class StoreController : Controller
    {
        DB_StoreEntities db = new DB_StoreEntities();
        private int recordsPerPage = 20;
        ErrorViewModel Error = new ErrorViewModel();
        [CustomAuthorize(Roles = "المخزن&edit")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getAll(int? page, string Search)
        {
            int? skipRecords = (page != null ? page.Value : 0) * recordsPerPage;

            var AllRecords = db.Stores
              .Where(s => string.IsNullOrEmpty(Search) ? true : s.Name.Contains(Search))
              .OrderBy(s => s.ID).Skip(skipRecords != null ? skipRecords.Value : 0)
              .Take(recordsPerPage).Select(a=>new Cls_Store{
                  ID=a.ID,
                  Name=a.Name
              }).ToList();
            var list = JsonConvert.SerializeObject(AllRecords,
 Formatting.None,
 new JsonSerializerSettings()
 {
     ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
     
 });
           

            return Content(list, "application/json");
        }
        [HttpGet]
        public ActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Insert(Store Store)
        {
            db.Stores.Add(Store);
            db.SaveChanges();
            Error.ErrorName = "تمت الحفظ بنجاح ... جاري إعادة تحميل الصفحة";
            return Json(Error, JsonRequestBehavior.AllowGet);
           

        }
        [HttpPost]
        public ActionResult Edit(Store Store)
        {
            Store item = db.Stores.Find(Store.ID);
            item.Name = Store.Name;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            Error.ErrorName = "تمت الحفظ بنجاح ... جاري إعادة تحميل الصفحة";
            return Json(Error, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Store Store = db.Stores.Find(id);
            db.Stores.Remove(Store);
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