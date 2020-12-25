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

namespace Warehouse.Controllers
{

    [CustomAuthorize(Roles = "عمولة مبيعات&save$1")]
    public class OmolaController : Controller
    {
        DB_StoreEntities db = new DB_StoreEntities();
        ErrorViewModel Error = new ErrorViewModel();


        [HttpGet]
        public ActionResult loadOmola()
        {
            Omola Omola = db.Omolas.FirstOrDefault();
            ClsOmola ClsOmola = new ClsOmola {
                ID=Omola.ID,
                Omola_quota=Omola.Omola_quota,
                Omola_Type=Omola.Omola_Type
            };
            var list = JsonConvert.SerializeObject(Omola, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
        }
        [HttpGet]
        
        public ActionResult Operation()
        {
            return View();
        }
      
        [HttpPost]
        public ActionResult Edit(Omola Omola)
        {
            db.Entry(Omola).State = EntityState.Modified;
            db.SaveChanges();
            Error.ErrorName = "تمت الحفظ بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = Omola.ID;
            return Json(Error, JsonRequestBehavior.AllowGet);
        }


        

    }
}