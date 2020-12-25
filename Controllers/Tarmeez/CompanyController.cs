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

namespace Warehouse.Controllers.Tarmeez
{
    public class CompanyController : Controller
    {
        DB_StoreEntities db = new DB_StoreEntities();
        private int recordsPerPage = 20;
        ErrorViewModel Error = new ErrorViewModel();
        public ActionResult getAll(int? page, string Search)
        {
            int? skipRecords = (page != null ? page.Value : 0) * recordsPerPage;

            var AllRecords = db.Companies
              .Where(s => string.IsNullOrEmpty(Search) ? true : s.Name.Contains(Search))
              .OrderBy(a => a.Name).Skip(skipRecords != null ? skipRecords.Value : 0)
              .Take(recordsPerPage).Select(a => new Cls_Company
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
        [CustomAuthorize(Roles = "اسم الشركة&save$1")]
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
        public ActionResult Edit(Company Company)
        {
            Company item = db.Companies.Find(Company.ID);
            item.Name = Company.Name;
            
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
        public ActionResult Insert(Company Company)
        {
            db.Companies.Add(Company);
            db.SaveChanges();
            Error.ErrorName = "تمت الحفظ بنجاح ... جاري إعادة تحميل الصفحة";
            return Json(Error, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Company Company = db.Companies.Find(id);
            db.Companies.Remove(Company);
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